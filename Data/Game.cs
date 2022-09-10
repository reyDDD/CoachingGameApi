using System.ComponentModel.DataAnnotations;

namespace TamboliyaApi.Data
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public bool IsFinished { get; set; } = false;
        public virtual InitialGameData InitialGameData { get; set; } = null!;
        public virtual ActualPositionOnTheMap ActualPosition { get; set; } = null!;
    }
}
