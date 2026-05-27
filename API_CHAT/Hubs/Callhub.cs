namespace API_CHAT.Hubs
{
    using Microsoft.AspNetCore.SignalR;

    public class CallHub : Hub
    {
        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }

        public async Task SendOffer(string roomId, string offer)
        {
            await Clients.OthersInGroup(roomId)
                .SendAsync("ReceiveOffer", offer);
        }

        public async Task SendAnswer(string roomId, string answer)
        {
            await Clients.OthersInGroup(roomId)
                .SendAsync("ReceiveAnswer", answer);
        }

        public async Task SendIce(string roomId, string ice)
        {
            await Clients.OthersInGroup(roomId)
                .SendAsync("ReceiveIce", ice);
        }

        public async Task EndCall(string roomId)
        {
            await Clients.OthersInGroup(roomId)
                .SendAsync("CallEnded");
        }
    }
}

