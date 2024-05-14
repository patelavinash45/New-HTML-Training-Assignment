using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IChatService
    {
        void AddChat(int senderId, int userId, string message);
    }
}
