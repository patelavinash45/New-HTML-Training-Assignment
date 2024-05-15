using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Npgsql;
using Services.Interfaces;
using Services.ViewModels;

namespace HelloDoc.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public Task SendPrivateMessage(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveMessage", message);
        }

        public async Task SendMessage(string message)
        {
            var httpContext = Context.GetHttpContext();
            int senderId = httpContext.Session.GetInt32("aspNetUserId").Value;
            int reciverId = httpContext.Session.GetInt32("receiverId").Value;
            await _chatService.AddChat(senderId, reciverId, message);
        }
    }

    public class BackgroundServices : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private string connectionString;
        private readonly IHubContext<ChatHub> _hubContext;

        public BackgroundServices(IConfiguration configuration, IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            _configuration = configuration;
            connectionString = _configuration["ConnectionStrings:ConnectionStrings"];
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();
            connection.Notification += ConnectionOnNotification;
            using var command = new NpgsqlCommand("LISTEN notifytickets", connection);
            command.ExecuteNonQuery();
            while (!stoppingToken.IsCancellationRequested)
            {
                await connection.WaitAsync();
            }
            await Task.CompletedTask;
        }

        private async void ConnectionOnNotification(object sender, NpgsqlNotificationEventArgs e)
        {
            try
            {
                ChatMessage data = JsonConvert.DeserializeObject<ChatMessage>(e.Payload);
                await _hubContext.Clients.User(data.SenderId).SendAsync("ReceiveMessage", data.Message);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
