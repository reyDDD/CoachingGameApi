using TamboliyaApi.GameLogic.Models;
using TamboliyaLibrary.DAL;

namespace TamboliyaApi.GameLogic.DAL
{
    public class GameDTO
    {
        public int GameId { get; set; }
        public bool IsFinished { get; set; }
        public ActualPositionOnMap ActualPosition { get; set; } = null!;
        public OracleDTO? Oracle { get; set; }
        public List<ActualPositionOnMap> ActualPositionsForSelect { get; set; } = new();
    }
}
