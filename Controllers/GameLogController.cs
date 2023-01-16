using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TamboliyaApi.Data;
using TamboliyaApi.Services;
using TamboliyaLibrary.DAL;
using TamboliyaLibrary.Models;


namespace TamboliyaApi.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class GameLogController : ControllerBase
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public GameLogController(UnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        /// <summary>
        /// Get game's log
        /// </summary>
        /// <param name="gameId">Input parameter with game's id</param>
        /// <returns>Status code 200 and model if reques is success</returns>
        [HttpGet]
        [Route("info")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<string>>> Info([FromQuery] int gameId)
        {
            if (gameId == default(int))
                return BadRequest("Input model isn't correct");

            var userId = await GetUserId();


            Game? userGame = (await unitOfWork.GameRepository.GetAsync(game => game.Id == gameId && game.CreatorGuid == userId)).FirstOrDefault();

            if (userGame == null)
                return BadRequest("User game not found");

            Game parentGame = null!;
            if (userGame!.GameType == GameType.Parrent)
            {
                parentGame = userGame;
            }
            else
            {
                parentGame = (await unitOfWork.GameRepository.GetAsync(game => game.Id == userGame.ParentGameId)).FirstOrDefault();
            }

            IEnumerable<Game> childGames = await unitOfWork.GameRepository.GetAsync(game => game.ParentGameId == parentGame.Id);

            IEnumerable<GameChatLog> logLines;

            if (childGames?.Count() != 0)
            {

                logLines = await unitOfWork.GameChatLog.GetAsync(filter: log => childGames.Select(x => x.Id).Contains(log.GameId) || log.GameId == parentGame.Id);
            }
            else
            {
                logLines = await unitOfWork.GameChatLog.GetAsync(filter: log => parentGame.Id == log.GameId);
            }

            if (logLines.Count() == 0)
            {
                return Ok(new List<string>());
            }

            return Ok(logLines.Select(mess => mess.Message));
        }


        /// <summary>
        /// Get game's log line
        /// </summary>
        /// <param name="gameLogLineId">Input parameter with game's log line id</param>
        /// <returns>Status code 200 and model if reques is success</returns>
        [HttpGet]
        [Route("gamesLogLine")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GameChatLogDTO>> GetGamesLogLine([FromQuery] int gameLogLineId)
        {
            var userId = await GetUserId();

            var logLine = await unitOfWork.GameChatLog.GetAsync(log => log.Id == gameLogLineId && log.UserId == userId);


            if (logLine.Count() == 0)
            {
                return NotFound();
            }
            var logLineDTOModel = _mapper.Map<GameChatLogDTO>(logLine.First());
            return Ok(logLineDTOModel);
        }

        //TODO: this controller cannot used. Why?
        /// <summary>
        /// Save game's log
        /// </summary>
        /// <param name="logsOfGame">List of logs with game's information</param>
        /// <returns>Status code 201 if all lines are saved</returns>
        /// <remarks>Sample request:
        /// </remarks>
        [HttpPost]
        [Route("save")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LogsDTOModel>> Save([FromBody] LogsDTOModel logsOfGame)
        {
            var userId = await GetUserId();

            if (logsOfGame.GameId == default(int))
            {
                ModelState.AddModelError("GameId", "Input parameter GameId is't valid");
            }
            if (logsOfGame!.UserId != userId)
            {
                ModelState.AddModelError("UserId", "User cannot access to selected game");
            }
            if (!ModelState.IsValid)
                return BadRequest("Input model isn't correct");


            var game = await unitOfWork.GameRepository.GetByIDAsync(game => game.Id == logsOfGame.GameId && game.CreatorGuid == userId);
            if (game == null)
                return BadRequest("Game not found");

            unitOfWork.GameChatLog.Insert(new GameChatLog
            {
                Game = game,
                UserId = logsOfGame.UserId,
                Message = logsOfGame.Message
            });

            await unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(Info), new { gameId = logsOfGame.GameId }, logsOfGame);
        }


        /// <summary>
        /// Add new message to game's log
        /// </summary>
        /// <param name="logLine">New model's message for log</param>
        /// <returns>Status code 201 if line are added</returns>
        [HttpPost]
        [Route("addMessageToLog")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddMessageToLog([FromBody] LogLineDTOModel logLine)
        {
            var userId = await GetUserId();

            if (String.IsNullOrEmpty(logLine!.Message))
            {
                ModelState.AddModelError("Message", "Message is null or empty");
            }
            if (logLine.GameId == default(int))
            {
                ModelState.AddModelError("GameId", "Input parameter GameId is't valid");
            }
            if (!ModelState.IsValid)
                return BadRequest("Input model isn't correct");


            var game = await unitOfWork.GameRepository.GetByIDAsync(game => game.Id == logLine.GameId && game.CreatorGuid == userId);
            if (game == null)
                return BadRequest("Game not found");

            var newLogLine = new GameChatLog
            {
                Game = game,
                UserId = userId,
                Message = logLine.Message
            };
            unitOfWork.GameChatLog.Insert(newLogLine);
            await unitOfWork.SaveAsync();

            var logLineDTOModel = _mapper.Map<GameChatLogDTO>(newLogLine);
            return CreatedAtAction(nameof(GetGamesLogLine), newLogLine.Id, logLineDTOModel);
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
