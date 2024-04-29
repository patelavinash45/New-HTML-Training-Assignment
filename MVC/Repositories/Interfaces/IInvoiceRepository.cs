using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Invoice GetAllInvoiceByPhysician(int aspNetUserId, DateTime startDate);
    }
}
