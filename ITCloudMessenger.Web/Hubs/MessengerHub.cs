using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ITCloudMessenger.Web.Hubs
{
    public class MessengerHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
