using Microsoft.AspNetCore.SignalR;

namespace ShowMyLifeAPI.HubMessage
{
    public class PostHub : Hub
    {
        public async Task SendPostNotification(string message)
        {
            await Clients.All.SendAsync("ReceivePostNotification", message);
        }
    }

}
