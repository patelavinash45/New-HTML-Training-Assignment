using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface ILogsService
    {
        Task<bool> AddEmailLog(EmailLog emailLog);

        List<EmailLog> GetAllEmailLogs(Func<EmailLog, bool> predicate);

        List<Smslog> GetAllSMSLogs(Func<Smslog, bool> predicate);
    }
}
