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
            return _dbContext.Invoices.Include(a => a.Physician).Include(a => a.InvoiceDetails).Include(a => a.Reimbursements)
                            .FirstOrDefault(a => a.Physician.AspNetUserId == aspNetUserId && a.StartDate == startDate);
        }

        public List<Reimbursement> GetAllReimbursementByPhysician(int aspNetUserId, DateOnly startDate, DateOnly endDate)
        {
            return _dbContext.Reimbursements.Include(a => a.Physician).Include(a => a.RequestWiseFile)
                     .Where(a => a.Physician.AspNetUserId == aspNetUserId && a.Date <= startDate && a.Date >= endDate).ToList();
        }

        public async Task<bool> AddInvoice(Invoice invoice)
        {
            _dbContext.Invoices.Add(invoice);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateInvoice(Invoice invoice)
        {
            _dbContext.Invoices.Update(invoice);
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

        public async Task<bool> UpdateInvoiceDetails(List<InvoiceDetail> invoiceDetails)
        {
            _dbContext.InvoiceDetails.UpdateRange(invoiceDetails);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateReimbursement(List<Reimbursement> reimbursements)
        {
            _dbContext.Reimbursements.UpdateRange(reimbursements);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
