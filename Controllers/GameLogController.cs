using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TamboliyaApi.Data;
using TamboliyaApi.Services;
using TamboliyaLibrary.DAL;

//TODO: використати для пеертворення моделей автомаппер
namespace TamboliyaApi.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class GameLogController : ControllerBase
    {
        private readonly UnitOfWork unitOfWork;

        public GameLogController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
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
        public async Task<ActionResult<LogsDTOModel>> Info([FromQuery] int gameId)
        {
            if (gameId == default(int))
                return BadRequest("Input model isn't correct");

            var userId = await GetUserId();

            var logs = await unitOfWork.GameChatLog.GetAsync(log => log.GameId == gameId && log.UserId == userId);

            if (logs.Count() == 0)
            {
                return Ok(logs.GameLogsToLogsDTOModel(new GameUserDTO() { GameId = gameId, UserId = userId }));
            }

            return Ok(logs.GameLogsToLogsDTOModel(new GameUserDTO() { GameId = gameId, UserId = userId }));
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

            return Ok(logLine.FirstOrDefault()?.GameChatLogToGameChatLogDTO());
        }


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
            if (!ModelState.IsValid)
                return BadRequest("Input model isn't correct");

            foreach (var item in logsOfGame.Messages)
            {
                unitOfWork.GameChatLog.Insert(new GameChatLog
                {
                    Game = (await unitOfWork.GameRepository.GetByIDAsync(game => game!.Id == logsOfGame!.GameId))!,
                    UserId = logsOfGame.UserId,
                    Message = item
                });
            }
            await unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(Info), new { gameId = logsOfGame.GameId, userId = logsOfGame.UserId }, logsOfGame);
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
            if (logLine.GameId == default(int)) {
                ModelState.AddModelError("GameId", "Input parameter GameId is't valid");
            }
            if (!ModelState.IsValid)
                return BadRequest("Input model isn't correct");

            var userId = await GetUserId();

            var newLogLine = new GameChatLog
            {
                Game = (await unitOfWork.GameRepository.GetByIDAsync(game => game.Id == logLine.GameId && game!.CreatorGuid == userId))!,
                UserId = userId,
                Message = logLine.Message
            };
            unitOfWork.GameChatLog.Insert(newLogLine);
            await unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetGamesLogLine), newLogLine.Id, newLogLine.GameChatLogToGameChatLogDTO());
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
