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
            Invoice invoice = _invoiceRepository.GetInvoiceByPhysician(aspNetUserId, startDate);
            CreateInvoice createInvoice = new CreateInvoice()
            {
                StartDate = startDate,
                ShiftHours = shiftHours,
            };
            if (invoice != null)
            {
                foreach(InvoiceDetail invoiceDetail in invoice.InvoiceDetails)
                {
                    createInvoice.TotalHours.Add(invoiceDetail.TotalHours);
                    createInvoice.NoOfPhoneConsults.Add(invoiceDetail.NumberOfHouseCall);
                    createInvoice.NoOfPhoneConsults.Add(invoiceDetail.NumberOfPhoneConsults);
                    if(invoiceDetail.IsHoliday)
                    {
                        createInvoice.IsHoliday.Add(invoiceDetail.Date.Day);
                    }
                }
                return new InvoicePage()
                {
                    Date = startDate,
                    StartDate = invoice.StartDate.ToString("MMM dd,yyyy"),
                    EndDate = invoice.EndDate.ToString("MMM dd,yyyy"),
                    Status = invoice.Status ? "Approved" : "Pending",
                    CreateInvoice = createInvoice
                };
            }
            return new InvoicePage()
            {
                Date = startDate,
                CreateInvoice = createInvoice
            };
        }

        public Receipts GetReceipts(int aspNetUserId,string date)
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
            Receipts receipts = new Receipts() 
            {
                StartDate = startDate,
            };
            List<Reimbursement> reimbursements = _invoiceRepository.GetAllReimbursementByPhysician(aspNetUserId, startDate);
            if(reimbursements.Count > 0)
            {
                foreach(Reimbursement reimbursement in reimbursements)
                {
                    receipts.Items.Add(reimbursement.Item);
                    receipts.Amounts.Add(reimbursement.Amount);
                    receipts.Path.Add(GetFile(aspNetUserId, reimbursement.Date.Day));
                }
            }
            return receipts;
        }

        private string GetFile(int aspNetUserId, int day)
        {
            String path = Path.Combine($"/Files//Invoice/{ aspNetUserId}/{day}");
            String _path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/Files//Invoice/{aspNetUserId}/{day}");
            FileInfo[] Files = new DirectoryInfo(_path).GetFiles().OrderBy(p => p.LastWriteTime).ToArray();
            return Path.Combine(path, Files[^1].Name);
        }
    }
}
