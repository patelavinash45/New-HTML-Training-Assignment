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

        public Invoice GetInvoiceByPhysician(int aspNetUserId, DateOnly startDate)
        {
            return _dbContext.Invoices.Include(a => a.Physician).Include(a => a.InvoiceDetails)
                        .FirstOrDefault(a => a.Physician.AspNetUserId == aspNetUserId && a.StartDate <= startDate && a.EndDate >= startDate.AddDays(15));
        }

        public List<Reimbursement> GetAllReimbursementByPhysician(int aspNetUserId, DateTime startDate)
        {
            return _dbContext.Reimbursements.Include(a => a.Physician).Include(a => a.RequestWiseFile)
                     .Where(a => a.Physician.AspNetUserId == aspNetUserId && a.Date <= startDate && a.Date >= startDate.AddDays(15)).ToList();
        }

        public async Task<bool> AddInvoice(Invoice invoice)
        {
            _dbContext.Invoices.Add(invoice);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        
        public async Task<bool> AddInvoiceDetails(List<InvoiceDetail> invoiceDetails)
        {
            _dbContext.InvoiceDetails.AddRange(invoiceDetails);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        
        public async Task<bool> AddReimbursement(List<Reimbursement> reimbursements)
        {
            _dbContext.Reimbursements.AddRange(reimbursements);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
