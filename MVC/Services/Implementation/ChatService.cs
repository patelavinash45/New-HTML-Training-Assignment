using Repositories.DataModels;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.ViewModels;

namespace Services.Implementation
{
    public class ChatService : IChatService
    {
        private readonly ILogsRepository _logsRepository;

        public ChatService(ILogsRepository logsRepository)
        {
            _logsRepository = logsRepository;
        }
        

        public async Task<bool> AddChat(int senderId, int requestId, string message, int type)
        {
            return await _logsRepository.AddChat(
                new Chat(){
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
                    Time = chat.Time.Value.ToString("h:mm tt"),
                    IsSend = chat.SenderId == aspNetUserId,
                  }).ToList();
        }
    }
}



