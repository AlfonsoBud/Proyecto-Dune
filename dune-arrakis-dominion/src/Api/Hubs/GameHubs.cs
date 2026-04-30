csharp src/Api/Hubs/GameHub.cs
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Api.Hubs
{
    public class GameHub : Hub
    {
        // Mensaje simple de broadcast
        public Task BroadcastGameUpdate(string gameJson)
        {
            return Clients.All.SendAsync("ReceiveGameUpdate", gameJson);
        }

        // Unirse / salir de grupo por gameId
        public Task JoinGameGroup(string gameId) => Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        public Task LeaveGameGroup(string gameId) => Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
    }
}