using System.Text.Json.Serialization;
using TamboliyaApi.Data;
using TamboliyaApi.GameLogic.Models;
using TamboliyaApi.Services;

namespace TamboliyaApi.GameLogic
{
    public class NewGame
    {


        public int Id { get; set; }
        public bool IsFinished { get; set; } = false;
        public Oracle Oracle { get; init; }
        public ActualPositionOnMap ActualPosition { get; set; } = null!;

        public List<ActualPositionOnMap> ActualPositionsForSelect { get; set; } = new();

        [JsonIgnore]
        public ProphecyCollectionService RedProphecies { get; init; }

        [JsonIgnore]
        public ProphecyCollectionService GreenProphecies { get; init; }

        [JsonIgnore]
        public ProphecyCollectionService BlueProphecies { get; init; }

        [JsonIgnore]
        public ProphecyCollectionService YellowProphecies { get; init; }


        private readonly ChooseRandomActionService chooseRandomAction;
        private readonly NewMoveService newMoveService;
        private readonly LogService logService;


        public NewGame(Oracle oracle,
            ChooseRandomActionService chooseRandomActionService,
            NewMoveService newMoveService,
            LogService logService)
        {
            RedProphecies = ProphecyCollectionService.Create(Color.Red);
            GreenProphecies = ProphecyCollectionService.Create(Color.Green);
            BlueProphecies = ProphecyCollectionService.Create(Color.Blue);
            YellowProphecies = ProphecyCollectionService.Create(Color.Yellow);
            Oracle = oracle;
            chooseRandomAction = chooseRandomActionService;
            this.newMoveService = newMoveService;
            this.logService = logService;
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
            ActualPosition = await newMoveService.MakeMoveAsync(ActualPosition, this, game);
        }

        public async Task ChooseRandomAction()
        {
            ActualPosition = await chooseRandomAction.ChooseAsync();
        }

        public async Task ChooseRandomCard(Game game)
        {
            _ = ActualPosition.RegionOnMap switch
            {
                RegionOnMap.OrganizationalPath => await GetPrompt(RedProphecies, game),
                RegionOnMap.PersonalPath => await GetPrompt(YellowProphecies, game),
                RegionOnMap.MysticalPath => await GetPrompt(BlueProphecies, game),
                RegionOnMap.InnerHomePath => await GetPrompt(GreenProphecies, game)
            };
        }

        private async Task<string> GetPrompt(ProphecyCollectionService prophecyService, Game game)
        {
            string prompt = await prophecyService.GetProphecyAsync();
            logService.AddRecord(game, prompt);



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
            else
            {
                throw new ArgumentException(prompt, nameof(prompt));
            }

            return prompt;


        }

        public async Task GoToNewPositionOnTheMap(RegionOnMap regionOnMap, int stepNumber)
        {
            ActualPositionOnMap newPosition = new();

            string pathToCards = regionOnMap switch
            {
                RegionOnMap.LandOfClarity => GamePathes.mapLandOfClarityPath,
                RegionOnMap.OrganizationalPath => GamePathes.mapOrganizationalPath,
                RegionOnMap.PersonalPath => GamePathes.mapPersonalPath,
                RegionOnMap.InnerHomePath => GamePathes.mapInnerHomePath,
                _ => throw new ArgumentException("RegionOnMap isn't correct", regionOnMap.ToString())
            };


            var rootFolder = Directory.GetCurrentDirectory();
            var path = Path.Combine(rootFolder, pathToCards);
            var prophecies = (await File.ReadAllLinesAsync(path)).ToList();
            string prophecy = prophecies.Where(m => m.StartsWith($"{stepNumber} — ")).First();

            newPosition.RegionOnMap = regionOnMap;
            newPosition.Description = prophecy;
            newPosition.PositionNumber = stepNumber;
        }





    }
}
