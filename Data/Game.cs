﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.Data
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTimeOffset DateBeginning { get; set; }

        public DateTimeOffset DateEnding { get; set; }

        [Required]
        public Guid CreatorGuid { get; set; } = new Guid();
        public bool IsFinished { get; set; } = false;
        public virtual InitialGameData InitialGameData { get; set; } = null!;
        public virtual ActualPositionOnTheMap ActualPosition { get; set; } = null!;

        [Required, Range(1,2)]
        public GameType GameType { get; set; }

        [Range(1,6)]
        public int MaxUsersCount { get; set; } = default(int);

        public virtual Game? ParentGame { get; set; }
        public int? ParentGameId { get; set; }

        public IEnumerable<Game>? ChildsGames = new List<Game>();


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
