using Services.ViewModels.Physician;

namespace Services.Interfaces.PhysicianServices
{
    public interface IInvoiceService
    {
        InvoicePage GetInvoice(int aspNetUserId, string startDate);

        CreateInvoice GetWeeklyTimeSheet(int aspNetUserId, DateTime startDate);

        Receipts GetReceipts(int aspNetUserId, string date);

        Task<bool> SaveInvoice(CreateInvoice model, int aspNetUserId, bool isFinalize);
    }
}
