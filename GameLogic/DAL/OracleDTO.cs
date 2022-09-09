﻿using TamboliyaApi.GameLogic.Models;

namespace TamboliyaApi.GameLogic.ModelDTOs
{
    public class OracleDTO
    {
        public int GameId { get; init; }
        public string Question { get; init; } = null!;
        public string Motive { get; init; } = null!;
        public string QualityOfExperience { get; init; } = null!;
        public string EnvironmentAndCircumstances { get; init; } = null!;
        public string ChainLinks { get; init; } = null!;
        public string ExitPath { get; init; } = null!;
        public int StepOnPath { get; init; }
        public RegionOnMap RegionOnMap { get; init; }
    }
}
