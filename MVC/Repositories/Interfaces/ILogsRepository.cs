using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface ILogsRepository
    {
        Task<bool> AddEmailLog(EmailLog emailLog);

        List<EmailLog> GetAllEmailLogs(Func<EmailLog, bool> predicate);

        List<Smslog> GetAllSMSLogs(Func<Smslog, bool> predicate);

        Task<bool> AddChat(Chat chat);

        List<Chat> GetChats(int requestId, int type);
    }
}
