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

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            int requestId = httpContext.Session.GetInt32("requestId").Value;
            int chatType = httpContext.Session.GetInt32("chatType").Value;
            string groupName;
            switch(chatType)
            {
                case 1: groupName = $"AdminPatient{requestId}"; break;
                case 2: groupName = $"AdminPhysician{requestId}"; break;
                default: groupName = $"PhysicianPatient{requestId}"; break; 
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            int requestId = httpContext.Session.GetInt32("requestId").Value;
            int chatType = httpContext.Session.GetInt32("chatType").Value;
            string groupName;
            switch(chatType)
            {
                case 1: groupName = $"AdminPatient{requestId}"; break;
                case 2: groupName = $"AdminPhysician{requestId}"; break;
                default: groupName = $"PhysicianPatient{requestId}"; break; 
            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            var httpContext = Context.GetHttpContext();
            int requestId = httpContext.Session.GetInt32("requestId").Value;
            int chatType = httpContext.Session.GetInt32("chatType").Value;
            string groupName;
            switch(chatType)
            {
                case 1: groupName = $"AdminPatient{requestId}"; break;
                case 2: groupName = $"AdminPhysician{requestId}"; break;
                default: groupName = $"PhysicianPatient{requestId}"; break; 
            }
            int aspNetUserId = httpContext.Session.GetInt32("aspNetUserId").Value;
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message, DateTime.Now.ToString("h:mm tt"), aspNetUserId.ToString());
            await _chatService.AddChat(aspNetUserId, requestId, message, chatType);
        }
    }

    // public class BackgroundServices : BackgroundService
    // {
    //     private readonly IConfiguration _configuration;
    //     private string connectionString;
    //     private readonly IServiceProvider _serviceProvider;
    //     private readonly IHubContext<ChatHub> _hubContext;

    //     public BackgroundServices(IConfiguration configuration, IServiceProvider serviceProvider, IHubContext<ChatHub> hubContext)
    //     {
    //         _configuration = configuration;
    //         _serviceProvider = serviceProvider;
    //         connectionString = _configuration["ConnectionStrings:ConnectionStrings"];
    //         _hubContext = hubContext;
    //     }

    //     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //     {
    //         NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    //         connection.Open();
    //         connection.Notification += ConnectionOnNotification;
    //         using var command = new NpgsqlCommand("LISTEN notifytickets", connection);
    //         command.ExecuteNonQuery();
    //         while (!stoppingToken.IsCancellationRequested)
    //         {
    //             await connection.WaitAsync();
    //         }
    //         await Task.CompletedTask;
    //     }

    //     private async void ConnectionOnNotification(object sender, NpgsqlNotificationEventArgs e)
    //     {
    //         try
    //         {   
    //             ChatMessage chatMessage = JsonConvert.DeserializeObject<ChatMessage>(e.Payload);
    //             using (var scope = _serviceProvider.CreateScope())
    //             {
    //                 var scopedServices = scope.ServiceProvider;
    //                 var chatHub = scopedServices.GetRequiredService<ChatHub>();
    //                 await chatHub.SetMessage(chatMessage, _hubContext);
    //             }
    //         }
    //         catch (Exception ex){}
    //     }
    // }
}
