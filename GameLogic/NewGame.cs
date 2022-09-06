using TamboliyaApi.Data;
using TamboliyaApi.Services;

namespace TamboliyaApi.GameLogic
{
    public class NewGame
    {
        public ProphecyCollectionService RedProphecies { get; init; }
        public ProphecyCollectionService GreenProphecies { get; init; }
        public ProphecyCollectionService BlueProphecies { get; init; }
        public ProphecyCollectionService YellowProphecies { get; init; }
        public Oracle Oracle { get; init; }


        public NewGame(Oracle oracle)
        {
            RedProphecies = ProphecyCollectionService.Create(Color.Red);
            GreenProphecies = ProphecyCollectionService.Create(Color.Green);
            BlueProphecies = ProphecyCollectionService.Create(Color.Blue);
            YellowProphecies = ProphecyCollectionService.Create(Color.Yellow);
            this.Oracle = oracle;
        }


        public void Start(string userQuestion)
        {
            GetOracle(userQuestion);
        }

        private void GetOracle(string userQuestion)
        {
            Oracle.Start(userQuestion);
        }
    }
}
