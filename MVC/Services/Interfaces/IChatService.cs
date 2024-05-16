using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IChatService
    {
        Task<bool> AddChat(int senderId, int requestId, string message, short type);

        List<ChatMessage> GetChat(int aspNetUserId, int requestId, int type);
    }
}
