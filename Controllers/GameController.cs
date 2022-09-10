﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TamboliyaApi.Data;
using TamboliyaApi.GameLogic;
using TamboliyaApi.GameLogic.DAL;
using TamboliyaApi.GameLogic.Models;
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
        private readonly LogService logService;

        public GameController(AppDbContext context, NewGame game,
            UnitOfWork unitOfWork, LogService logService)
        {
            this.context = context;
            this.newGame = game;
            this.unitOfWork = unitOfWork;
            this.logService = logService;
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
            logService.AddOracle(game);
            return CreatedAtAction(nameof(StartNewGame), game.InitialGameData.InitialGameDataToOracleDTO());
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
        public ActionResult<GameDTO> GetInfoAboutGame(int gameId)
        {
            var actualGame = unitOfWork.GameRepository
                .Get(game => game.Id == gameId, includeProperties: "ActualPosition,InitialGameData").FirstOrDefault();

            if (actualGame == null) return new BadRequestObjectResult($"Game with id {gameId} not found");

            var gameDTO = actualGame!.GameToGameDTO()!;
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
        ///     "UserId" : "0000-0000-0000-0000-0000",
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

            var game = unitOfWork.GameRepository
                .GetByID(game => game.Id == moveModel.GameId,
                includeProperties: "ActualPosition,InitialGameData");
            if (game == null) return BadRequest("Game not found");
            if (game.IsFinished) return BadRequest("Game was finished");

            newGame.ActualPosition = game.ActualPosition.ActualPositionOnMapToDTO();


            if ((moveModel.RegionOnMap != RegionOnMap.NotSet) &&
                (moveModel.PositionNumber != null && moveModel.PositionNumber != 0))
            {
                await newGame.GoToNewPositionOnTheMap(moveModel.RegionOnMap,
                    moveModel.PositionNumber!.Value);
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

                if (newGame.ActualPosition!.CompareBaseFiedls(game.ActualPosition))
                {
                    await newGame.GoToNewStage(game);
                    if (newGame.ActualPositionsForSelect.Count() > 0)
                    {
                        return CreatedAtAction(nameof(MakeMove), game.GameToGameDTO(newGame.ActualPositionsForSelect));
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
            await unitOfWork.SaveAsync();
            logService.AddRecord(game);


            if (game.IsFinished)
            {
                await newGame.EndOfTheGame(game);
            }

            return CreatedAtAction(nameof(MakeMove), game.GameToGameDTO(newGame.ActualPositionsForSelect));
        }
    }
}
