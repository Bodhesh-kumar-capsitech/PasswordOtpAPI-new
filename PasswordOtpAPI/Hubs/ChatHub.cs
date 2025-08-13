using Microsoft.AspNetCore.SignalR;

namespace PasswordOtpAPI.Hubs
{
    public class ChatHub:Hub
    {

        public async Task Sendmessage(string user,string message)
        {
            await Clients.All.SendAsync("Received" , user , message);
        }
    }
}
