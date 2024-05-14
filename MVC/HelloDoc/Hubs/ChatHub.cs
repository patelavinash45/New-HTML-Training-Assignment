using Microsoft.AspNetCore.SignalR;
using Services.Interfaces;

namespace HelloDoc.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task SendMessage(string message)
        {
            var httpContext = Context.GetHttpContext();
            int senderId = httpContext.Session.GetInt32("aspNetUserId").Value;
            int userId = httpContext.Session.GetInt32("userId").Value;
            _chatService.AddChat(senderId, userId, message);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
