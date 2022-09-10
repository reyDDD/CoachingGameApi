using TamboliyaApi.Data;

namespace TamboliyaApi.GameLogic.Models
{
    public class ActualPositionOnMap
    {
        public RegionOnMap RegionOnMap { get; set; }
        public string Description { get; set; } = null!;
        public int PositionNumber { get; set; }
        public bool? IsSelected { get; set; }

        public bool CompareBaseFiedls(ActualPositionOnTheMap positionAtBase)
        {
            if (this.RegionOnMap == positionAtBase.RegionOnMap &&
                this.Description == positionAtBase.Description &&
                this.PositionNumber == positionAtBase.PositionNumber)
            {

                return true;
            }
            return false;
        }
    }


}
