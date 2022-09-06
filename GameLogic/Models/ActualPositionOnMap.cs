namespace TamboliyaApi.GameLogic.Models
{
    public class ActualPositionOnMap
    {
        public RegionOnMap RegionOnMap { get; set; }
        public string Description { get; set; } = null!;
        public int PositionNumber { get; set; }
    }
}
