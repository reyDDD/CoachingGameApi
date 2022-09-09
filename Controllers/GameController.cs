using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TamboliyaApi.Data;
using TamboliyaApi.GameLogic;
using TamboliyaApi.Services;

namespace TamboliyaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly NewGame newGame;
        private readonly UnitOfWork unitOfWork;

        public GameController(AppDbContext context, NewGame game,
            UnitOfWork unitOfWork)
        {
            this.context = context;
            this.newGame = game;
            this.unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Start new game
        /// </summary>
        /// <param name="question">User question</param>
        /// <returns>The result of the oracle</returns>
        [HttpGet]
        [Route("new")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StartNewGame(string question)
        {
            var game = (await newGame.GetOracle(question)).NewGameToGame();
            unitOfWork.GameRepository.Insert(game);
            await unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(StartNewGame), game.InitialGameData.InitialGameDataToOracleDTO());
        }
    }
}
