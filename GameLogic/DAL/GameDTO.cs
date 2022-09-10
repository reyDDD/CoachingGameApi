﻿using TamboliyaApi.Data;
using TamboliyaApi.GameLogic.ModelDTOs;
using TamboliyaApi.GameLogic.Models;

namespace TamboliyaApi.GameLogic.DAL
{
    public class GameDTO
    {
        public int GameId { get; set; }
        public bool IsFinished { get; set; }
        public ActualPositionOnMap ActualPosition { get; set; } = null!;
        public OracleDTO? Oracle { get; set; }
        public List<ActualPositionOnMap> ActualPositionsForSelect { get; set; } = new();
    }
}
