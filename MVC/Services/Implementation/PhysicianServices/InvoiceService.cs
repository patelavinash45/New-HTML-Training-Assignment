using DocumentFormat.OpenXml.Wordprocessing;
using Repositories.DataModels;
using Repositories.Interfaces;
using Services.Interfaces.PhysicianServices;
using Services.ViewModels.Physician;

namespace Services.Implementation.PhysicianServices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public InvoicePage GetInvoice(int aspNetUserId)
        {
            DateTime startDate = DateTime.Now;
            if(startDate.Day < 15)
            {
                startDate.AddDays(1 - startDate.Day);
            }
            else
            {
                startDate.AddDays(15 - startDate.Day);
            }
            Invoice invoice = _invoiceRepository.GetAllInvoiceByPhysician(aspNetUserId, startDate);
            if(invoice != null) 
            {
                return new InvoicePage()
                {
                    Date = startDate,
                    StartDate = invoice.StartDate.Value.ToString("MMM dd,yyyy"),
                    EndDate = invoice.EndDate.Value.ToString("MMM dd,yyyy"),
                    Status = invoice.Status.Value ? "Approved" : "Pending",
                };
            }
            return new InvoicePage()
            {
                Date = startDate,
            };
        }
    }
}
