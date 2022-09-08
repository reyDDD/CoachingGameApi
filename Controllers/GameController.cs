using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TamboliyaApi.Data;
using TamboliyaApi.GameLogic;

namespace TamboliyaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly NewGame game;

        public GameController(AppDbContext context, NewGame game)
        {
            this.context = context;
            this.game = game;
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
            var newGame = await game.GetOracle(question);

            return CreatedAtAction(nameof(StartNewGame), newGame);
        }
    }
}
