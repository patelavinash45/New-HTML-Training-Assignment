using Microsoft.Extensions.Configuration;
using Repositories.DataModels;
using Repositories.Interfaces;
using Services.Interfaces;
using Npgsql;
using Services.ViewModels;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace Services.Implementation
{
    public class ChatService : IChatService
    {
        private readonly ILogsRepository _logsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private string connectionString;

        public ChatService(ILogsRepository logsRepository, IUserRepository userRepository, IConfiguration configuration)
        {
            _logsRepository = logsRepository;
            _userRepository = userRepository;
            _configuration = configuration;
            connectionString = _configuration["ConnectionStrings:ConnectionStrings"];
        }

        public async Task<bool> AddChat(int senderId, int reciverId, string message)
        {
            await _logsRepository.AddChat(new Chat()
            {
                SenderId = senderId,
                ReceiverId = reciverId,
                Message = message,
                Time = DateTime.Now,
            });
            return true;
        }
    }
}



