using Repositories.DataModels;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementation
{
    public class ChatService : IChatService
    {
        private readonly ILogsRepository _logsRepository;
        private readonly IUserRepository _userRepository;

        public ChatService(ILogsRepository logsRepository, IUserRepository userRepository)
        {
            _logsRepository = logsRepository;
            _userRepository = userRepository;
        }

        public void AddChat(int senderId, int userId, string message)
        {
            User user = _userRepository.GetUserByUserId(userId);
            _logsRepository.AddChat(new Chat()
            {
                SenderId = senderId,
                ReceiverId = user.AspNetUserId,
                Message = message,
                Time = DateTime.Now,
            });
        }
    }
}
