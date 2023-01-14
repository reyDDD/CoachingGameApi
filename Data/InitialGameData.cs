using System.ComponentModel.DataAnnotations;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.Data
{
    public class InitialGameData
    {
        [Key]
        public int Id { get; set; } = 0;
        public string Question { get; set; } = null!;
        public string Motive { get; set; } = null!;
        public string QualityOfExperience { get; set; } = null!;
        public string EnvironmentAndCircumstances { get; set; } = null!;
        public string ChainLinks { get; set; } = null!;
        public string ExitPath { get; set; } = null!;
        public RegionOnMap RegionOnMap { get; set; } = RegionOnMap.NotSet;
        public int StepOnPath { get; set; } = 0;

        public int GameId { get; set; } = 0;
        public Game Game { get; set; } = null!;
    }
}
