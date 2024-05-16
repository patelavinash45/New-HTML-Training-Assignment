using Microsoft.Extensions.Configuration;
using Repositories.DataModels;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.ViewModels;

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
        

        public async Task<bool> AddChat(int senderId, int requestId, string message, short type)
        {
            return await _logsRepository.AddChat(new Chat()
            {
                SenderId = senderId,
                RequestId = requestId,
                Message = message,
                Time = DateTime.Now,
                Type = type,
            });
        }

        public List<ChatMessage> GetChat(int aspNetUserId, int requestId, int type)
        {
            return _logsRepository.GetChats(requestId, type)
                  .Select(chat => new ChatMessage(){
                    Message = chat.Message,
                    Time = chat.Time.Value.ToString("hh:MM"),
                    IsSend = chat.SenderId == aspNetUserId,
                  }).ToList();
        }
    }
}



