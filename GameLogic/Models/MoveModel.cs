namespace TamboliyaApi.GameLogic.Models
{
    public class MoveModel
    {
        public int GameId { get; set; }
        public ActionType? ActionType { get; set; }
        public Guid? UserId { get; set; }

        public RegionOnMap? RegionOnMap { get; set; }
        public int? PositionNumber { get; set; }
    }
}
