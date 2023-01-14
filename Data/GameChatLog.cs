using System.ComponentModel.DataAnnotations;

namespace TamboliyaApi.Data
{
    public class GameChatLog
    {
		[Key]
		public int Id { get; set; }
		[Required]
		public Guid UserId { get; set; }
        [Required]
        public string Message { get; set; } = null!;

		public int GameId { get; set; }
		public Game Game { get; set; } = null!;
	}
}
