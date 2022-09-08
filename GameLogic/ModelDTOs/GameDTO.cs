using TamboliyaApi.GameLogic.Models;
using TamboliyaApi.Services;

namespace TamboliyaApi.GameLogic.ModelDTOs
{
    public class GameDTO
    {
        public int Id { get; set; }
        public bool IsFinished { get; set; } = false;
        public ActualPositionOnMap ActualPosition { get; set; } = null!;
        public Queue<string> PromptMessages { get; set; } = new Queue<string>();
        public List<ActualPositionOnMap> ActualPositionsForSelect { get; init; } = null!;
        public Oracle Oracle { get; init; } = null!;

        public ProphecyCollectionService RedProphecies { get; init; } = null!;
        public ProphecyCollectionService GreenProphecies { get; init; } = null!;
        public ProphecyCollectionService BlueProphecies { get; init; } = null!;
        public ProphecyCollectionService YellowProphecies { get; init; } = null!;
    }
}
