using TamboliyaApi.Data;
using TamboliyaApi.GameLogic.Models;
using TamboliyaApi.Services;

namespace TamboliyaApi.GameLogic
{
    public class NewGame
    {
        public bool IsFinished { get; set; } = false;
        public ProphecyCollectionService RedProphecies { get; init; }
        public ProphecyCollectionService GreenProphecies { get; init; }
        public ProphecyCollectionService BlueProphecies { get; init; }
        public ProphecyCollectionService YellowProphecies { get; init; }
        public Oracle Oracle { get; init; }
        public RegionOnMap RegionOnMap { get; set; }
        public ChooseRandomActionService chooseRandomAction { get; set; }
        public NewMoveService newMoveService { get; set; }
        public ActualPositionOnMap ActualPosition { get; set; } = null!;
        public Queue<string> PromptMessages { get; set; } = new Queue<string>();
        public List<ActualPositionOnMap> ActualPositionsForSelect { get; set; } = new();


        public NewGame(Oracle oracle,
            ChooseRandomActionService chooseRandomActionService,
            NewMoveService newMoveService)
        {
            RedProphecies = ProphecyCollectionService.Create(Color.Red);
            GreenProphecies = ProphecyCollectionService.Create(Color.Green);
            BlueProphecies = ProphecyCollectionService.Create(Color.Blue);
            YellowProphecies = ProphecyCollectionService.Create(Color.Yellow);
            this.Oracle = oracle;
            chooseRandomAction = chooseRandomActionService;
            this.newMoveService = newMoveService;
        }



        public async Task GetOracle(string userQuestion)
        {
            await Oracle.Start(userQuestion);
        }

        public async Task GoToNewStage()
        {
            ActualPosition = await newMoveService.MakeMoveAsync(ActualPosition, this);
        }

        public async Task ChooseRandomAction()
        {
            ActualPosition = await chooseRandomAction.ChooseAsync();
        }

        public async Task ChooseRandomCard()
        {
            var prompt = RegionOnMap switch
            {
                RegionOnMap.OrganizationalPath => await GetPrompt(RedProphecies),
                RegionOnMap.PersonalPath => await GetPrompt(YellowProphecies),
                RegionOnMap.MysticalPath => await GetPrompt(BlueProphecies),
                RegionOnMap.InnerHomePath => await GetPrompt(GreenProphecies),
                _ => throw new ArgumentException("Region on the map is not right")
            };

            PromptMessages.Enqueue(prompt);
        }

        private async Task<string> GetPrompt(ProphecyCollectionService prophecyService)
        {
            return await prophecyService.GetProphecyAsync();
        }







    }
}
