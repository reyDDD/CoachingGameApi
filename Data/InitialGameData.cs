using System.ComponentModel.DataAnnotations;
using TamboliyaApi.GameLogic.Models;

namespace TamboliyaApi.Data
{
    public class InitialGameData
    {
        [Key]
        public int Id { get; set; }
        public string Question { get; private set; } = null!;
        public string Motive { get; private set; } = null!;
        public string QualityOfExperience { get; private set; } = null!;
        public string EnvironmentAndCircumstances { get; private set; } = null!;
        public string ChainLinks { get; private set; } = null!;
        public string ExitPath { get; private set; } = null!;
        public RegionOnMap RegionOnMap { get; private set; }
        public int StepOnPath { get; private set; }

        public int GameId { get; set; }
        public Game Game { get; set; } = null!;
    }
}
