using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using TamboliyaApi.Data;
using TamboliyaApi.GameLogic;
using TamboliyaApi.Services;
using TamboliyaLibrary.DAL;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.Controllers
{
    [Authorize(Roles ="User")]
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly NewGame newGame;
        private readonly UnitOfWork unitOfWork;
        private readonly LogService logService;
        private readonly ILogger<GameController> logger;

        public GameController(AppDbContext context, NewGame game,
            UnitOfWork unitOfWork, LogService logService, ILogger<GameController> logger)
        {
            this.context = context;
            this.newGame = game;
            this.unitOfWork = unitOfWork;
            this.logService = logService;
            this.logger = logger;
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

            var game = (await this.newGame.GetOracle(newGame, userGuid)).NewGameToGame();
            unitOfWork.GameRepository.Insert(game);
            await logService.AddOracle(game, this.newGame);
            await unitOfWork.SaveAsync();

            OracleDTO? DTO = default;
            try
            {
                DTO = game.InitialGameData.InitialGameDataToOracleDTO();
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

            var gameDTO = actualGame!.GameToGameDTO()!;
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

            var gameDTO = userGames!.Select(x => x.GameToGameDTO()!);
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
        public async Task<ActionResult<IEnumerable<GameDTO>>> GetLastGamesToJoin()
        {
            var userGuid = await GetUserId();
            //TODO: зробити так, щоб в списку не показувались ігри, до яких користувач уже приєднався раніше (він в списку дочірніх гравців)
            var userGames = (await unitOfWork.GameRepository
                .GetAsync(
                filter: u => u.CreatorGuid != userGuid && u.IsFinished != true && u.DateBeginning > DateTime.UtcNow,
                includeProperties: "ActualPosition,InitialGameData",
                orderBy: x => x.OrderBy(x => x.DateBeginning)))
                .Take(20);

            if (userGames.Count() == 0) return new BadRequestObjectResult("User games not found");

            var gameDTO = userGames!.Select(x => x.GameToGameDTO()!);
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
            newGame.ActualPosition = game.ActualPosition.ActualPositionOnMapToDTO();

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
                        return CreatedAtAction(nameof(MakeMove), moveModel.GameId, game.GameToGameDTO(newGame.ActualPositionsForSelect));
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

            return CreatedAtAction(nameof(MakeMove), moveModel.GameId, game.GameToGameDTO(newGame.ActualPositionsForSelect));
        }


        /// <summary>
        /// Endpoint to end the game at the player's request
        /// </summary>
        /// <param name="gameId">Game id</param>
        /// <returns>Success status code if all is good</returns>
        [HttpPost]
        [Route("finish")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> FinishTheGame([FromBody] int gameId)
        {
            var game = await unitOfWork.GameRepository
                .GetByIDAsync(game => game.Id == gameId,
                includeProperties: "ActualPosition,InitialGameData");
            if (game == null) return BadRequest("Game not found");
            if (game.IsFinished) return BadRequest("The game has already been completed");

            await newGame.EndOfTheGame(game);

            game.IsFinished = true;
            try
            {
                unitOfWork.GameRepository.Update(game);
                await unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
    }
}
