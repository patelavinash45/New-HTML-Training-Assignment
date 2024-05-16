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
            string role = httpContext.Session.GetString("role");
            if(role == "Admin")
            {
                string groupName = $"AdminPatient{requestId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
            else
            {
                string groupName = $"AdminPatient{requestId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, string message)
        {
            var httpContext = Context.GetHttpContext();
            int requestId = httpContext.Session.GetInt32("requestId").Value;
            string groupName = $"AdminPatient{requestId}";
            int aspNetUserId = httpContext.Session.GetInt32("aspNetUserId").Value;
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message, DateTime.Now.ToString("hh: MM"), aspNetUserId.ToString());
            await _chatService.AddChat(aspNetUserId, requestId, message, 1);
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
