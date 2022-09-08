using System.ComponentModel.DataAnnotations.Schema;
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
        public Queue<string> PromptMessages { get; set; } = new Queue<string>();

        public List<ActualPositionOnMap> ActualPositionsForSelect { get; set; } = new();

        [JsonIgnore]
        public ProphecyCollectionService RedProphecies { get; init; }

        [JsonIgnore]
        public ProphecyCollectionService GreenProphecies { get; init; }

        [JsonIgnore]
        public ProphecyCollectionService BlueProphecies { get; init; }

        [JsonIgnore]
        public ProphecyCollectionService YellowProphecies { get; init; }

        [JsonIgnore]
        public ChooseRandomActionService chooseRandomAction { get; set; }

        [JsonIgnore]
        public NewMoveService NewMoveService { get; set; }



        public NewGame(Oracle oracle,
            ChooseRandomActionService chooseRandomActionService,
            NewMoveService newMoveService)
        {
            RedProphecies = ProphecyCollectionService.Create(Color.Red);
            GreenProphecies = ProphecyCollectionService.Create(Color.Green);
            BlueProphecies = ProphecyCollectionService.Create(Color.Blue);
            YellowProphecies = ProphecyCollectionService.Create(Color.Yellow);
            Oracle = oracle;
            chooseRandomAction = chooseRandomActionService;
            NewMoveService = newMoveService;
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

        public async Task GoToNewStage()
        {
            ActualPosition = await NewMoveService.MakeMoveAsync(ActualPosition, this);
        }

        public async Task ChooseRandomAction()
        {
            ActualPosition = await chooseRandomAction.ChooseAsync();
        }

        public async Task ChooseRandomCard()
        {
            _ = ActualPosition.RegionOnMap switch
            {
                RegionOnMap.OrganizationalPath => await GetPrompt(RedProphecies),
                RegionOnMap.PersonalPath => await GetPrompt(YellowProphecies),
                RegionOnMap.MysticalPath => await GetPrompt(BlueProphecies),
                RegionOnMap.InnerHomePath => await GetPrompt(GreenProphecies),
                _ => throw new ArgumentException("Region on the map is not right")
            };
        }

        private async Task<string> GetPrompt(ProphecyCollectionService prophecyService)
        {
            string prompt = await prophecyService.GetProphecyAsync();
            PromptMessages.Enqueue(prompt);

            ActualPositionOnMap newPosition = new();

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
                await GoToNewPositionOnTheMap(RegionOnMap.LandOfClarity,
                    GamePathes.mapLandOfClarityPath, (int)LandOfClarity.Gatekeeper);
            }
            else if (prompt == whatAmIDoingHere)
            {
                await GoToNewPositionOnTheMap(RegionOnMap.LandOfClarity,
                    GamePathes.mapLandOfClarityPath, (int)LandOfClarity.WhatAmIDoingHere);
            }
            else if (prompt.Contains(goToOrganizationalGrowth))
            {
                await GoToNewPositionOnTheMap(RegionOnMap.OrganizationalPath,
                    GamePathes.mapOrganizationalPath, (int)OrganizationalPath.Start);
            }
            else if (prompt.Contains(goToPersonalGrowth))
            {
                await GoToNewPositionOnTheMap(RegionOnMap.PersonalPath,
                    GamePathes.mapPersonalPath, (int)PersonalPath.Birth);
            }
            else if (prompt.Contains(goToInnerHome))
            {
                await GoToNewPositionOnTheMap(RegionOnMap.InnerHomePath,
                    GamePathes.mapInnerHomePath, (int)InnerHome.HealthyBody);
            }
            else
            {
                throw new ArgumentException(prompt, nameof(prompt));
            }
            PromptMessages.Enqueue(newPosition.Description);
            return prompt;

            async Task GoToNewPositionOnTheMap(RegionOnMap regionOnMap, string pathToCards, int stepNumber)
            {
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
}
