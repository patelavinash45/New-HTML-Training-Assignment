using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Invoice GetInvoiceByPhysician(int aspNetUserId, DateTime startDate);

        List<Reimbursement> GetAllReimbursementByPhysician(int aspNetUserId, DateTime startDate);
    }
}
