using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TamboliyaApi.Data;
using TamboliyaApi.GameLogic;
using TamboliyaApi.GameLogic.DAL;
using TamboliyaApi.GameLogic.Models;
using TamboliyaApi.Services;
using TamboliyaLibrary.DAL;
using TamboliyaLibrary.Models;

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
			logService.AddOracle(game);
			await unitOfWork.SaveAsync();
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
		public async Task<ActionResult<GameDTO>> GetInfoAboutGame(int gameId)
		{
			var actualGame = (await unitOfWork.GameRepository
				.GetAsync(game => game.Id == gameId, includeProperties: "ActualPosition,InitialGameData")).FirstOrDefault();

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
				moveModel.ActionType != ActionType.NewStep)
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

				if (newGame.ActualPosition!.CompareBaseFiedls(game.ActualPosition))
				{
					await newGame.GoToNewStage(game);
					if (newGame.ActualPositionsForSelect.Count() > 0)
					{
						await unitOfWork.SaveAsync();
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

			if (game.IsFinished)
			{
				await newGame.EndOfTheGame(game);
			}

			logService.AddRecord(game);
			await unitOfWork.SaveAsync();

			return CreatedAtAction(nameof(MakeMove), game.GameToGameDTO(newGame.ActualPositionsForSelect));
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
		public async Task<ActionResult<string>> FinishTheGame(int gameId)
		{
			var game = await unitOfWork.GameRepository
				.GetByIDAsync(game => game.Id == gameId,
				includeProperties: "ActualPosition,InitialGameData");
			if (game == null) return BadRequest("Game not found");
			if (game.IsFinished) return BadRequest("The game has already been completed");

			await newGame.EndOfTheGame(game);

			game.IsFinished = true;
			unitOfWork.GameRepository.Update(game);
			await unitOfWork.SaveAsync();
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
			var log = await unitOfWork.GameLog.GetAsync(logs => logs.GameId == gameId,
				orderBy: q => q.OrderBy(d => d.Id));
			var logCollection = log.Select(x => x.Message).ToList();

			if (logCollection.Count == 0) return BadRequest("Game not found");
			else
			{
				return Ok(new GameLogDTO { Messages = logCollection });
			}
		}
	}
}
