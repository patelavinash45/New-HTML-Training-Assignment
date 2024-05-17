using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IChatService
    {
        Task<bool> AddChat(int senderId, int requestId, string message, int type);

        List<ChatMessage> GetChat(int aspNetUserId, int requestId, int type);
    }
}
