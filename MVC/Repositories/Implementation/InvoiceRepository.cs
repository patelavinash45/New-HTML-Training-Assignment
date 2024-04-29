using Microsoft.EntityFrameworkCore;
using Repositories.DataContext;
using Repositories.DataModels;
using Repositories.Interfaces;

namespace Repositories.Implementation
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly HalloDocDbContext _dbContext;

        public InvoiceRepository(HalloDocDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Invoice GetAllInvoiceByPhysician(int aspNetUserId, DateTime startDate)
        {
            return _dbContext.Invoices.Include(a => a.Physician).FirstOrDefault(a => a.Physician.AspNetUserId == aspNetUserId &&
                                                                a.StartDate <= startDate && a.EndDate >= startDate.AddDays(15));
        }
    }
}
