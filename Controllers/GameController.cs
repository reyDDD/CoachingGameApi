using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using System.Globalization;
using System.Linq;
using TamboliyaApi.Data;
using TamboliyaApi.GameLogic;
using TamboliyaApi.Services;
using TamboliyaLibrary.DAL;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly NewGame newGame;
        private readonly UnitOfWork unitOfWork;
        private readonly LogService logService;
        private readonly ILogger<GameController> logger;
        private readonly IMapper _mapper;


        public GameController(AppDbContext context, NewGame game,
            UnitOfWork unitOfWork, LogService logService, ILogger<GameController> logger,
            IMapper mapper)
        {
            this.context = context;
            this.newGame = game;
            this.unitOfWork = unitOfWork;
            this.logService = logService;
            this.logger = logger;
            this._mapper = mapper;

        }


        /// <summary>
        /// Start new game
        /// </summary>
        /// <param name="newGame">Model for creating new parent game</param>
        /// <returns>The result of the oracle</returns>
        [HttpPost]
        [Route("new")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StartNewGame([FromBody] NewUserGame newGame)
        {
            var userGuid = await GetUserId();

            if (newGame.GameType == GameType.Child)
            {
                var parentGame = (await unitOfWork.GameRepository.GetAsync(game =>
                game.Id == newGame.ParentGame)).FirstOrDefault();
                if (parentGame == null)
                {
                    return BadRequest("Батьківська гра, до якої намагається приєднатися користувач, не існує");
                }

                var childGames = await unitOfWork.GameRepository.GetAsync(game =>
                game.ParentGameId == newGame.ParentGame, includeProperties: "ParentGame");

                var countChildGameAtParent = childGames?.Count() ?? 0;
                var maxCountChildGames = parentGame.MaxUsersCount;

                if (countChildGameAtParent >= maxCountChildGames)
                {
                    return BadRequest("Автор обмежив кількість гравців, які можуть приєднатися до поточної гри");
                }
            }

            var game = await this.newGame.GetOracle(newGame, userGuid);
            var gameCasted = _mapper.Map<Game>(game);
            unitOfWork.GameRepository.Insert(gameCasted);
            await logService.AddOracle(gameCasted, this.newGame);
            await unitOfWork.SaveAsync();

            OracleDTO? DTO = default;
            try
            {
                DTO = _mapper.Map<OracleDTO>(gameCasted.InitialGameData);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error from model converter");
            }
            return CreatedAtAction(nameof(StartNewGame), DTO);
        }

        /// <summary>
        /// Get info about game
        /// </summary>
        /// <param name="gameId">Game Id</param>
        /// <returns>Info about game</returns>
        [HttpGet]
        [Route("info")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GameDTO>> GetInfoAboutGame(int gameId)
        {
            var userGuid = await GetUserId();

            var actualGame = (await unitOfWork.GameRepository
                .GetAsync(game => game.Id == gameId && game.CreatorGuid == userGuid, includeProperties: "ActualPosition,InitialGameData")).FirstOrDefault();

            if (actualGame == null) return new BadRequestObjectResult($"Game with id {gameId} not found");

            var gameDTO = _mapper.Map<GameDTO>(actualGame!);

            return Ok(gameDTO);
        }


        [SwaggerOperation(
            Summary = "Get info about user games",
            Description = "Return user collection of games")]
        [HttpGet]
        [Route("gamesInfo")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<GameDTO>>> GetInfoAboutGames()
        {
            var userGuid = await GetUserId();

            var userGames = (await unitOfWork.GameRepository
                .GetAsync(u => u.CreatorGuid == userGuid, includeProperties: "ActualPosition,InitialGameData"));

            if (userGames.Count() == 0) return new BadRequestObjectResult("User games not found");

            var gameDTO = userGames!.Select(x => _mapper.Map<GameDTO>(x!));

            return Ok(gameDTO);
        }


        [SwaggerOperation(
            Summary = "Get list latest games to join",
            Description = "Return collection of latest games to join")]
        [HttpGet]
        [Route("lastGamesToJoin")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<GameDTO>>> GetLastGamesToJoin(string dateBeginning, string dateEnding, int? offset)
        {
            if (String.IsNullOrEmpty(dateBeginning) && String.IsNullOrEmpty(dateEnding) && !offset.HasValue)
            {
                return BadRequest("One or few input parameters isn't valid");
            }

            DateTime _dateBeginning = DateTime.Parse(dateBeginning, CultureInfo.InvariantCulture);
            DateTime _dateEnding = DateTime.Parse(dateEnding, CultureInfo.InvariantCulture);

            var startDate = GameExtensions.ConverLocalDateToDateOffset(_dateBeginning, offset!.Value);
            var endDate = GameExtensions.ConverLocalDateToDateOffset(_dateEnding, offset!.Value);

            var userGuid = await GetUserId();
            var parentUserGames = (await unitOfWork.GameRepository
                .GetAsync(
                filter: u => u.CreatorGuid != userGuid &&
                u.IsFinished != true &&
                u.DateBeginning >= startDate &&
                u.DateEnding <= endDate &&
                u.GameType == GameType.Parrent,

                includeProperties: "ActualPosition,InitialGameData",
                orderBy: x => x.OrderBy(x => x.DateBeginning)))
                .Take(20)
                .ToList();

            List<Game> childGames = new List<Game>();

            if (parentUserGames.Count() == 0)
                return new BadRequestObjectResult("User games not found");
            else
            {
                childGames = (await unitOfWork.GameRepository
                .GetAsync(
                filter: u => u.CreatorGuid == userGuid &&
                u.GameType == GameType.Child &&
                u.ParentGame != null && parentUserGames!.Contains(u.ParentGame),

                includeProperties: "ParentGame"))
                .ToList();
            }

            if (childGames.Count() > 0)
            {
                parentUserGames = parentUserGames.Where(g => childGames.All(u => u.ParentGameId != g.Id)).ToList();
            }

            if (parentUserGames.Count() == 0)
                return BadRequest("User games not found");

            var gameDTO = parentUserGames!.Select(x => _mapper.Map<GameDTO>(x!));
            return Ok(gameDTO);
        }

        /// <summary>
        /// Go to next step (make a move)
        /// </summary>
        /// <param name="moveModel">Model for take next step at game</param>
        /// <returns>DTO model with new position at the game</returns>
        /// <remarks>Sample request:
        /// {
        ///     "GameId": 1,
        ///     "ActionType": 2,
        ///     "UserId" : "000000-0000-0000-0000-0000000000",
        ///     "RegionOnMap": 0,
        ///     "PositionNumber" : null
        /// }
        /// </remarks>
        [HttpPost]
        [Route("move")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GameDTO>> MakeMove([FromBody] MoveModel moveModel)
        {
            if (moveModel.ActionType <= ActionType.NotSet ||
                moveModel.ActionType > ActionType.GoToSelectPosition)
                return BadRequest("Action Type isn't correct");

            var game = await unitOfWork.GameRepository
                .GetByIDAsync(game => game.Id == moveModel.GameId,
                includeProperties: "ActualPosition,InitialGameData");
            if (game == null) return BadRequest("Game not found");
            if (game.IsFinished) return BadRequest("Game was finished");

            logService.AddRecord(game, $"Гравець зробив новий хід. Зона на карті {moveModel.RegionOnMap.ToString()}, номер нової позиції - {moveModel.PositionNumber}, спосіб пересування - {moveModel.ActionType}");
            newGame.ActualPosition = _mapper.Map<ActualPositionOnMap>(game.ActualPosition);

            if (game.ActualPosition.RegionOnMap == RegionOnMap.Embodiment &&
                moveModel.ActionType != ActionType.NewStep ||
                game.ActualPosition.RegionOnMap == RegionOnMap.LandOfClarity &&
                moveModel.ActionType == ActionType.RandomAction
                )
            {
                return BadRequest("В цій області карти рух далі можливий виключно існуючою дорогою");
            }
            if (game.ActualPosition.RegionOnMap == RegionOnMap.LandOfClarity &&
                game.ActualPosition.PositionNumber == (int)LandOfClarity.WhatAmIDoingHere &&
                !(moveModel.ActionType == ActionType.NewStep || moveModel.ActionType == ActionType.GoToSelectPosition) &&
                (moveModel.RegionOnMap != RegionOnMap.LandOfClarity ||

                (moveModel.RegionOnMap == RegionOnMap.Embodiment &&
                !(moveModel.PositionNumber!.Value is 1 || moveModel.PositionNumber!.Value is 3 ||
                moveModel.PositionNumber!.Value is 5)) ||

                moveModel.RegionOnMap != RegionOnMap.Embodiment)
                )
            {
                return BadRequest("В цій області карти рух далі можливий виключно існуючою дорогою далі по " +
                    "зоні Втілення або в будь-яку вибрану позицію на Землі Ясності");
            }

            if
                ((game.ActualPosition.RegionOnMap == RegionOnMap.InnerHomePath ||
                game.ActualPosition.RegionOnMap == RegionOnMap.Delusion ||
                game.ActualPosition.RegionOnMap == RegionOnMap.MysticalPath ||
                game.ActualPosition.RegionOnMap == RegionOnMap.OrganizationalPath ||
                game.ActualPosition.RegionOnMap == RegionOnMap.PersonalPath) && game.ActualPosition.PositionNumber == 12 && moveModel.ActionType != ActionType.NewStep)
            {
                return BadRequest("З цієї точки можливий перехід на слідуючий крок - конкретну позицію на Землі Ясності. Кидати кость або довільно перміщуватись не можна");
            }

            if ((game.ActualPosition.RegionOnMap == RegionOnMap.InnerHomePath ||
                game.ActualPosition.RegionOnMap == RegionOnMap.Delusion ||
                game.ActualPosition.RegionOnMap == RegionOnMap.MysticalPath ||
                game.ActualPosition.RegionOnMap == RegionOnMap.OrganizationalPath ||
                game.ActualPosition.RegionOnMap == RegionOnMap.PersonalPath) && game.ActualPosition.PositionNumber == 12)
            {
                await newGame.GoToNewStage(game);
            }
            else if (moveModel.ActionType == ActionType.GoToSelectPosition &&
                moveModel.RegionOnMap != RegionOnMap.NotSet &&
                moveModel.PositionNumber.HasValue && moveModel.PositionNumber > 0)
            {
                await newGame.GoToNewPositionOnTheMap(moveModel.RegionOnMap,
                    moveModel.PositionNumber.Value);
            }
            else if (moveModel.ActionType == ActionType.RandomAction)
            {
                await newGame.ChooseRandomAction();
            }
            else if (moveModel.ActionType == ActionType.NewStep)
            {
                if (!(game.ActualPosition.RegionOnMap == RegionOnMap.LandOfClarity ||
                    game.ActualPosition.RegionOnMap == RegionOnMap.Embodiment))
                {
                    await newGame.ChooseRandomCard(game);
                }

                if (game.CompareBaseFiedls(newGame.ActualPosition))
                {
                    await newGame.GoToNewStage(game);
                    if (newGame.ActualPositionsForSelect.Count() > 0)
                    {
                        await unitOfWork.SaveAsync();

                        var gameDTOInner = _mapper.Map<GameDTO>(game!);
                        gameDTOInner.ActualPositionsForSelect = newGame!.ActualPositionsForSelect;

                        return CreatedAtAction(nameof(MakeMove), moveModel.GameId, gameDTOInner);
                    }
                }
            }
            else
            {
                return BadRequest("Branch not implemented");
            }

            game.IsFinished = newGame.IsFinished;
            game.ActualPosition.RegionOnMap = newGame.ActualPosition.RegionOnMap;
            game.ActualPosition.Description = newGame.ActualPosition.Description;
            game.ActualPosition.PositionNumber = newGame.ActualPosition.PositionNumber;

            unitOfWork.GameRepository.Update(game);

            if (game.IsFinished)
            {
                await newGame.EndOfTheGame(game);
            }

            logService.AddRecord(game);
            await unitOfWork.SaveAsync();

            var gameDTOBase = _mapper.Map<GameDTO>(game!);
            gameDTOBase.ActualPositionsForSelect = newGame!.ActualPositionsForSelect;

            return CreatedAtAction(nameof(MakeMove), moveModel.GameId, gameDTOBase);
        }


        /// <summary>
        /// Endpoint to end the game at the player's request
        /// </summary>
        /// <param name="endingGame">Id game end offset</param>
        /// <returns>Success status code if all is good</returns>
        [HttpPost]
        [Route("finish")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> FinishTheGame([FromBody] EndGameDTO endingGame)
        {

            var userGuid = await GetUserId();

            var game = await unitOfWork.GameRepository
                .GetByIDAsync(game => game.Id == endingGame.GameId && game.CreatorGuid == userGuid,
                includeProperties: "ActualPosition,InitialGameData");
            var childsGames = await GetChildGames(endingGame.GameId);

            if (game == null) return BadRequest("Game not found");
            if (game.IsFinished) return BadRequest("The game has already been completed");

            await newGame.EndOfTheGame(game);
            game.IsFinished = true;
            unitOfWork.GameRepository.Update(game);

            //TODO: не удается завершить игру, получаю ошибку!!!! Списка дочерних игр не существует
            if (game.GameType == GameType.Parrent)
            {
                foreach (var childsGame in childsGames)
                {
                    if (!childsGame.IsFinished)
                    {
                        childsGame!.IsFinished = true;
                        unitOfWork.GameRepository.Update(childsGame);
                    }
                }
            }
            try
            {
                await unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.Message);
                return BadRequest("The game isn't update. There is error during update games data");
            }

            return Ok("Гру закінчено");
        }


        /// <summary>
        /// Get history of game
        /// </summary>
        /// <param name="gameId">Game ID</param>
        /// <returns>History of game</returns>
        [HttpGet]
        [Route("log/{gameId}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GameLogDTO>> GameLog(int gameId)
        {
            var userGuid = await GetUserId();
            var log = await unitOfWork.GameLog.GetAsync(logs => logs.GameId == gameId && logs.UserId == userGuid,
                orderBy: q => q.OrderByDescending(d => d.Id));
            var logCollection = log.Select(x => x.Message).ToList();

            if (logCollection.Count == 0) return BadRequest("Game not found");
            else
            {
                return Ok(new GameLogDTO { Messages = logCollection });
            }
        }

        /// <summary>
        /// Get status of game
        /// </summary>
        /// <param name="gameId">Game ID</param>
        /// <returns>Status of game</returns>
        [HttpGet]
        [Route("status/{gameId}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> GetGameStatus(int gameId)
        {
            if (gameId == default)
                return BadRequest("Game Id is not valid");

            var userGuid = await GetUserId();
            var games = await unitOfWork.GameRepository.GetAsync(game => game.Id == gameId && game.CreatorGuid == userGuid);
            var game = games.FirstOrDefault();
            if (game == null) return BadRequest("Game created by this user not found");
            else
            {
                return Ok(game!.IsFinished);
            }
        }

        private async Task<Guid> GetUserId()
        {
            var userMail = User!.Identity!.Name;
            var userId = (await unitOfWork.GameUsers.GetAsync(u => u.Email == userMail))
                .First()
                .Id;
            return Guid.Parse(userId);
        }

        private async Task<List<Game>> GetChildGames(int parentGameId)
        {
            var userGuid = await GetUserId();

            List<Game> childGames = (await unitOfWork.GameRepository
            .GetAsync(
            filter: u => u.CreatorGuid != userGuid &&
            u.GameType == GameType.Child &&
            u.ParentGameId == parentGameId,

            includeProperties: "ParentGame"))
            .ToList();


            if (childGames.Count() > 0)
            {
                return childGames;
            }
            else
            {
                return new();
            }
        }
    }
}
