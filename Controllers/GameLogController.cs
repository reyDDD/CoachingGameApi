using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TamboliyaApi.Data;
using TamboliyaApi.GameLogic;
using TamboliyaApi.Services;
using TamboliyaLibrary.DAL;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.Controllers
{
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
		public async Task<ActionResult<LogsDTOModel>> Info(int gameId, Guid? userId)
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
	}
}
