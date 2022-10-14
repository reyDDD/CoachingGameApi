namespace TamboliyaApi.Data
{
    public class GameChatLog
    {
		public int Id { get; set; }
		public Guid? UserId { get; set; }
		public string Message { get; set; } = null!;

		public int GameId { get; set; }
		public Game Game { get; set; } = null!;
	}
}
