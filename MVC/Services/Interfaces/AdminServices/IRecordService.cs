using Services.ViewModels.Admin;
using System.Data;

namespace Services.Interfaces.AdminServices
{
    public interface IRecordService
    {
        Records GetRecords(Records model);

        EmailSmsLogs GetEmailLog(EmailSmsLogs model);

        List<EmailSmsLogTableData> GetEmailLogTabledata(EmailSmsLogs model);

        EmailSmsLogs GetSMSlLog(EmailSmsLogs model);

        List<EmailSmsLogTableData> GetSMSLogTabledata(EmailSmsLogs model);

        DataTable ExportAllRecords();   

        PatientHistory GetPatientHistory(PatientHistory model, int pageNo);

        PatientHistoryTable GetPatientHistoryTable(string data, int pageNo);

        PatientRecord GetPatientRecord(int userId, int pageNo);

        BlockHistory GetBlockHistory(BlockHistory model, int pageNo);

        BlockHistoryTable GetBlockHistoryTable(string data, int pageNo, string Date);

        Task<bool> UnblockRequest(int requestId);
    }
}
