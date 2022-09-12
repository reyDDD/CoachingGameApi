using System.ComponentModel.DataAnnotations;
using TamboliyaApi.GameLogic.Models;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.Data
{
    public class ActualPositionOnTheMap
    {
        [Key]
        public int Id { get; set; }
        public RegionOnMap RegionOnMap { get; set; }
        public string Description { get; set; } = null!;
        public int PositionNumber { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; } = null!;
    }
}
