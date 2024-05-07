using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Invoice GetInvoiceByPhysician(int aspNetUserId, DateOnly startDate);

        List<Reimbursement> GetAllReimbursementByPhysician(int aspNetUserId, DateTime startDate);

        Task<bool> AddInvoice(Invoice invoice);

        Task<bool> AddInvoiceDetails(List<InvoiceDetail> invoiceDetails);

        Task<bool> AddReimbursement(List<Reimbursement> reimbursements);
    }
}
