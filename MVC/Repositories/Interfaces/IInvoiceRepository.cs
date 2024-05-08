using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Invoice GetInvoiceByPhysician(int aspNetUserId, DateOnly startDate);

        List<Reimbursement> GetAllReimbursementByPhysician(int aspNetUserId, DateOnly startDate, DateOnly endDate);

        Task<bool> AddInvoice(Invoice invoice);

        Task<bool> UpdateInvoice(Invoice invoice);

        Task<bool> AddInvoiceDetails(List<InvoiceDetail> invoiceDetails);

        Task<bool> UpdateInvoiceDetails(List<InvoiceDetail> invoiceDetails);

        Task<bool> AddReimbursement(List<Reimbursement> reimbursements);

        Task<bool> UpdateReimbursement(List<Reimbursement> reimbursements);
    }
}
