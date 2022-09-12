using System.Text.Json.Serialization;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.GameLogic.Models
{
    public class MoveModel
    {
        public int GameId { get; set; }
        public Guid? UserId { get; set; }
        public ActionType ActionType { get; set; }
        public RegionOnMap RegionOnMap { get; set; }
        public int? PositionNumber { get; set; }
    }
}
