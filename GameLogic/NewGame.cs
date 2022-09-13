﻿using TamboliyaApi.Data;
using TamboliyaApi.GameLogic.Models;
using TamboliyaApi.Services;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.GameLogic
{
	public class NewGame
	{
		public int Id { get; set; }
		public bool IsFinished { get; set; } = false;
		public Oracle Oracle { get; init; }
		public ActualPositionOnMap ActualPosition { get; set; } = null!;

		public List<ActualPositionOnMap> ActualPositionsForSelect { get; set; } = new();
		public ProphecyCollectionService prophecyService;


		private readonly ChooseRandomActionService chooseRandomAction;
		private readonly NewMoveService newMoveService;
		private readonly LogService logService;


		public NewGame(Oracle oracle,
			ChooseRandomActionService chooseRandomActionService,
			NewMoveService newMoveService, LogService logService,
			ProphecyCollectionService prophecyCollectionService)
		{
			Oracle = oracle;
			chooseRandomAction = chooseRandomActionService;
			this.newMoveService = newMoveService;
			this.logService = logService;
			this.prophecyService = prophecyCollectionService;
		}



		public async Task<NewGame> GetOracle(string userQuestion)
		{
			await Oracle.Start(userQuestion);
			ActualPosition = new()
			{
				Description = Oracle.ExitPath,
				PositionNumber = Oracle.StepOnPath,
				RegionOnMap = Oracle.RegionOnMap
			};

			return this;
		}

		public async Task GoToNewStage(Game game)
		{
			ActualPosition = await newMoveService.MakeMoveAsync(this, game);
		}

		public async Task ChooseRandomAction()
		{
			ActualPosition = await chooseRandomAction.ChooseAsync();
		}

		public async Task EndOfTheGame(Game game)
		{
			logService.AddRecord(game, $"Гравець вирішив закінчити гру на даному етапі");
			Task emotions = GetPrompt(Color.Green, game, false);
			Task deception = GetPrompt(Color.Yellow, game, false);
			Task equilibrium = GetPrompt(Color.Blue, game, false);
			Task desire = GetPrompt(Color.Red, game, false);

			await Task.WhenAll(emotions, deception, equilibrium, desire);
		}

		public async Task ChooseRandomCard(Game game)
		{
			_ = game.ActualPosition.RegionOnMap switch
			{
				RegionOnMap.OrganizationalPath => await GetPrompt(Color.Red, game),
				RegionOnMap.PersonalPath => await GetPrompt(Color.Yellow, game),
				RegionOnMap.MysticalPath => await GetPrompt(Color.Blue, game),
				RegionOnMap.Delusion => await GetPrompt(Color.Blue, game),
				RegionOnMap.InnerHomePath => await GetPrompt(Color.Green, game),
				_ => throw new ArgumentException("RegionOnMap isn't valid", ActualPosition.RegionOnMap.ToString())
			};
		}

		private async Task<string> GetPrompt(Color color,
			Game game, bool executeInstruction = true)
		{
			string prompt = await prophecyService.GetProphecyAsync(color);
			logService.AddRecord(game, @$"Гравець витягнув нову карточку з підказкою: {prompt}");

			if (executeInstruction)
			{
				await FollowInstructionsOnCard(prompt);
			}
			return prompt;
		}

		private async Task FollowInstructionsOnCard(string prompt)
		{
			string throwDice = "Брось игральную кость";
			string gateToLandOfClarity = "Отправляйся к воротам на Земле Ясности";
			string whatAmIDoingHere = "Отправляйся к вопросу Что я здесь делаю?";
			string goToOrganizationalGrowth = "Отправляйся к Организационному росту";
			string goToPersonalGrowth = "Отправляйся к Личностному Росту";
			string goToInnerHome = "Отправляйся к Внутреннему Дому";

			if (prompt == throwDice)
			{
				await ChooseRandomAction();
			}
			else if (prompt == gateToLandOfClarity)
			{
				await GoToNewPositionOnTheMap(RegionOnMap.LandOfClarity, (int)LandOfClarity.Gatekeeper);
			}
			else if (prompt == whatAmIDoingHere)
			{
				await GoToNewPositionOnTheMap(RegionOnMap.LandOfClarity, (int)LandOfClarity.WhatAmIDoingHere);
			}
			else if (prompt.Contains(goToOrganizationalGrowth))
			{
				await GoToNewPositionOnTheMap(RegionOnMap.OrganizationalPath, (int)OrganizationalPath.Start);
			}
			else if (prompt.Contains(goToPersonalGrowth))
			{
				await GoToNewPositionOnTheMap(RegionOnMap.PersonalPath, (int)PersonalPath.Birth);
			}
			else if (prompt.Contains(goToInnerHome))
			{
				await GoToNewPositionOnTheMap(RegionOnMap.InnerHomePath, (int)InnerHome.HealthyBody);
			}
		}

		public async Task GoToNewPositionOnTheMap(RegionOnMap regionOnMap, int stepNumber)
		{
			string pathToCards = regionOnMap switch
			{
				RegionOnMap.LandOfClarity => GamePathes.mapLandOfClarityPath,
				RegionOnMap.OrganizationalPath => GamePathes.mapOrganizationalPath,
				RegionOnMap.PersonalPath => GamePathes.mapPersonalPath,
				RegionOnMap.InnerHomePath => GamePathes.mapInnerHomePath,
				RegionOnMap.Embodiment => GamePathes.mapEmbodimentPath,
				RegionOnMap.Delusion => GamePathes.mapDelusionPath,
				_ => throw new ArgumentException("RegionOnMap isn't correct", regionOnMap.ToString())
			};


			var rootFolder = Path.Combine(Directory.GetCurrentDirectory()!, GamePathes.Prefix);
			var path = Path.Combine(rootFolder, pathToCards);
			var prophecies = (await File.ReadAllLinesAsync(path)).ToList();
			string prophecy = prophecies.Where(m => m.StartsWith($"{stepNumber} — ")).First();

			int separatorIndex = (prophecy.IndexOf('—')) != -1 ? prophecy.IndexOf('—') + 2 : 0;
			var textMessage = prophecy.AsSpan()[separatorIndex..(prophecy.Length - 1)].ToString();

			ActualPosition.RegionOnMap = regionOnMap;
			ActualPosition.Description = textMessage;
			ActualPosition.PositionNumber = stepNumber;
		}





	}
}
