using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
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
		/// <param name="gameId">Game id</param>
		/// /// <param name="userId">User id</param>
		/// <returns>Status code 200 and model if reques is success</returns>
		[HttpGet]
		[Route("info")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<LogsDTOModel>> Info(int gameId, int userId)
		{
			if (gameId == default(int))
				return BadRequest("Input model isn't correct");

			var logs = await unitOfWork.GameChatLog.GetAsync(log => log.GameId == gameId && log.UserId == userId);

			if (logs.Count() == 0) {
				return NotFound();
			}

			return Ok(logs.GameLogsToLogsDTOModel(gameId, userId));
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

			return CreatedAtAction(nameof(Info), new { gameId = logsOfGame.GameId, userId = logsOfGame.UserId}, logsOfGame);
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
            //TODO: проверить, что входящий параметр ид пользователя и игры действительный, иначе добавлять сообщение в моделстейт
            if (!ModelState.IsValid)
                return BadRequest("Input model isn't correct");

                unitOfWork.GameChatLog.Insert(new GameChatLog
                {
                    Game = (await unitOfWork.GameRepository.GetByIDAsync(game => game!.Id == logLine!.GameId))!,
                    UserId = logLine.UserId,
                    Message = logLine.Message
                });

            await unitOfWork.SaveAsync();
            //TODO: Доработать возвращаемое значение - сделать ГЕТ контроллер с правильными входными данными
            return CreatedAtAction(nameof(Info), new { gameId = logLine.GameId, userId = logLine.UserId }, logLine);
        }
    }
}
