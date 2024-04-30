using Repositories.DataModels;
using Repositories.Interfaces;
using Services.Interfaces.PhysicianServices;
using Services.ViewModels.Physician;

namespace Services.Implementation.PhysicianServices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IShiftRepository _shiftRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository, IShiftRepository shiftRepository)
        {
            _invoiceRepository = invoiceRepository;
            _shiftRepository = shiftRepository;
        }

        public InvoicePage GetInvoice(int aspNetUserId)
        {
            DateTime startDate = DateTime.Now;
            if (startDate.Day < 15)
            {
                startDate = startDate.AddDays(1 - startDate.Day);
            }
            else
            {
                startDate = startDate.AddDays(15 - startDate.Day);
            }
            Dictionary<int, double> shiftHours = new Dictionary<int, double>();
            _shiftRepository.GetShiftDetailByPhysicianIdAndDate(aspNetUserId, startDate, startDate.AddDays(14))
                .ForEach(shiftDetail =>
                {
                    double shiftHour = (shiftDetail.EndTime - shiftDetail.StartTime).TotalHours;
                    if (shiftHours.ContainsKey(shiftDetail.ShiftDate.Day))
                    {
                        shiftHours[shiftDetail.ShiftDate.Day] += shiftHour;
                    }
                    else
                    {
                        shiftHours.Add(shiftDetail.ShiftDate.Day, shiftHour);
                    }
                });
            CreateInvoice createInvoice = new CreateInvoice()
            {
                StartDate = startDate,
                ShiftHours = shiftHours,
            };
            Invoice invoice = _invoiceRepository.GetAllInvoiceByPhysician(aspNetUserId, startDate);
            if (invoice != null)
            {
                return new InvoicePage()
                {
                    Date = startDate,
                    StartDate = invoice.StartDate.Value.ToString("MMM dd,yyyy"),
                    EndDate = invoice.EndDate.Value.ToString("MMM dd,yyyy"),
                    Status = invoice.Status.Value ? "Approved" : "Pending",
                    CreateInvoice = createInvoice
                };
            }
            return new InvoicePage()
            {
                Date = startDate,
                CreateInvoice = createInvoice
            };
        }
    }
}
