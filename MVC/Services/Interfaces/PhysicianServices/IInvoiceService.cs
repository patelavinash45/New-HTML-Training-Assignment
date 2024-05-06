using Services.ViewModels.Physician;

namespace Services.Interfaces.PhysicianServices
{
    public interface IInvoiceService
    {
        InvoicePage GetInvoice(int aspNetUserId, DateTime startDate);

        Receipts GetReceipts(int aspNetUserId, string date);

        //Task<bool> CreateInvoice(CreateInvoice createInvoice);
    }
}
