using Microsoft.AspNetCore.SignalR;

namespace TamboliyaApi.Hubs
{
	public class ChatHub : Hub
	{
		public async Task SendMessage(string user, string message)
		{
			await Clients.All.SendAsync("ReceiveMessage", user, message);
		}

		public async Task SendMessageToGroup(int roomId, string message)
		{
			await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", roomId, message);
		}

		public async Task JoinRoom(int roomId)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
			await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", roomId, Context?.User?.Identity?.Name + " joined.");
		}

		public async Task LeaveRoom(int roomId)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
			await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", roomId, Context?.User?.Identity?.Name + " removed.");
		}
	}
}
