using System.ComponentModel.DataAnnotations;
using TamboliyaApi.GameLogic.Models;

namespace TamboliyaApi.Data
{
    public class InitialGameData
    {
        [Key]
        public int Id { get; set; }
        public string Question { get; set; } = null!;
        public string Motive { get; set; } = null!;
        public string QualityOfExperience { get; set; } = null!;
        public string EnvironmentAndCircumstances { get; set; } = null!;
        public string ChainLinks { get; set; } = null!;
        public string ExitPath { get; set; } = null!;
        public RegionOnMap RegionOnMap { get; set; }
        public int StepOnPath { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; } = null!;
    }
}
