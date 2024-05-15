using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IChatService
    {
        Task<bool> AddChat(int senderId, int reciverId, string message);
    }
}
