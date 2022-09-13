using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.Data
{
	public class Game
	{
		[Key]
		public int Id { get; set; }
		public Guid? UserId { get; set; } = new Guid();
		public bool IsFinished { get; set; } = false;
		public virtual InitialGameData InitialGameData { get; set; } = null!;
		public virtual ActualPositionOnTheMap ActualPosition { get; set; } = null!;

		public bool CompareBaseFiedls(ActualPositionOnMap positionAtBase)
		{
			if (ActualPosition.RegionOnMap == positionAtBase.RegionOnMap &&
				ActualPosition.Description == positionAtBase.Description &&
				ActualPosition.PositionNumber == positionAtBase.PositionNumber)
			{
				return true;
			}
			return false;
		}
	}


}
