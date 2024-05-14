using Microsoft.AspNetCore.Http;
using Repositories.DataModels;
using Repositories.Interfaces;
using Services.Interfaces.PhysicianServices;
using Services.ViewModels.Physician;
using System.Collections;

namespace Services.Implementation.PhysicianServices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IShiftRepository _shiftRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRequestWiseFileRepository _requestWiseFileRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository, IShiftRepository shiftRepository, IUserRepository userRepository, IRequestWiseFileRepository requestWiseFileRepository)
        {
            _invoiceRepository = invoiceRepository;
            _shiftRepository = shiftRepository;
            _userRepository = userRepository;
            _requestWiseFileRepository = requestWiseFileRepository;
        }

        public InvoicePage GetInvoice(int aspNetUserId, string date)
        {
            DateTime startDate = new DateTime();
            if (date == null)
            {
                startDate = DateTime.Now;
            }
            else
            {
                startDate = DateTime.Parse(date);
            }
            Dictionary<DateTime, DateTime> dates = new Dictionary<DateTime, DateTime>();
            if (startDate.Day < 15)
            {
                startDate = startDate.AddDays(1 - startDate.Day);
            }
            else
            {
                startDate = startDate.AddDays(15 - startDate.Day);
                int daysInMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
                DateTime firstPartStart = new DateTime(startDate.Year, startDate.Month, 1);
                DateTime secondPartEnd = new DateTime(startDate.Year, startDate.Month, daysInMonth);
                dates.Add(firstPartStart, secondPartEnd);
            }
            for (int i = -1; i > -4; i--)
            {
                DateTime currentDate = startDate.AddMonths(i);
                int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                DateTime firstPartStart = new DateTime(currentDate.Year, currentDate.Month, 1);
                DateTime secondPartEnd = new DateTime(currentDate.Year, currentDate.Month, daysInMonth);
                dates.Add(firstPartStart, secondPartEnd);
            }
            Invoice invoice = _invoiceRepository.GetInvoiceByPhysician(aspNetUserId, DateOnly.FromDateTime(startDate));
            if (invoice != null)
            {
                return new InvoicePage()
                {
                    StartDate = invoice.StartDate,
                    EndDate = invoice.EndDate,
                    Dates = dates,
                    Status = invoice.Status ? "Approve" : "Pending",
                };
            }
            return new InvoicePage()
            {
                StartDate = DateOnly.FromDateTime(startDate),
                Dates = dates,
            };
        }

        public CreateInvoice GetWeeklyTimeSheet(int aspNetUserId, DateTime startDate)
        {
            int totaldays = 14;
            if (startDate.Day < 15)
            {
                startDate = startDate.AddDays(1 - startDate.Day);
            }
            else
            {
                startDate = startDate.AddDays(15 - startDate.Day);
                totaldays = DateTime.DaysInMonth(startDate.Year, startDate.Month) - totaldays;
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
                StartDate = DateOnly.FromDateTime(startDate),
                ShiftHours = shiftHours,
            };
            Invoice invoice = _invoiceRepository.GetInvoiceByPhysician(aspNetUserId, DateOnly.FromDateTime(startDate));
            if (invoice != null)
            {
                createInvoice.InvoiceId = invoice.InvoiceId;
                PhysicianPayRate physicianPayRate = _userRepository.GetPhysicianPayRateByAspNetUserId(aspNetUserId);
                createInvoice.PayRates = new List<double>{physicianPayRate.Shift.Value,physicianPayRate.NightShiftWeekend.Value,physicianPayRate.HouseCall.Value,physicianPayRate.PhoneConsults.Value};
                foreach (InvoiceDetail invoiceDetail in invoice.InvoiceDetails)
                {
                    createInvoice.TotalHours.Add(invoiceDetail.TotalHours);
                    createInvoice.TotalOfShift += (invoiceDetail.TotalHours * createInvoice.PayRates[0]);
                    createInvoice.NoOfHouseCall.Add(invoiceDetail.NumberOfHouseCall);
                    createInvoice.TotalOfhouseCall += (invoiceDetail.NumberOfHouseCall * createInvoice.PayRates[2]);
                    createInvoice.NoOfPhoneConsults.Add(invoiceDetail.NumberOfPhoneConsults);
                    createInvoice.TotalOfPhone += (invoiceDetail.NumberOfPhoneConsults * createInvoice.PayRates[3]);
                    if (invoiceDetail.IsHoliday)
                    {
                        createInvoice.TotalOfweekend += createInvoice.PayRates[3];
                        createInvoice.IsHoliday.Add(invoiceDetail.Date.Day);
                    }
                }
                createInvoice.TotalAmount = createInvoice.TotalOfShift + createInvoice.TotalOfhouseCall + 
                                                  createInvoice.TotalOfPhone + createInvoice.TotalOfweekend;
            }
            else
            {
                createInvoice.TotalHours = Enumerable.Repeat(0.0, totaldays).ToList();
                createInvoice.NoOfPhoneConsults = Enumerable.Repeat(0, totaldays).ToList();
                createInvoice.NoOfHouseCall = Enumerable.Repeat(0, totaldays).ToList();
            }
            return createInvoice;
        }

        public Receipts GetReceipts(int aspNetUserId, string date)
        {
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Parse(date));
            int totaldays = 14;
            if (startDate.Day < 15)
            {
                startDate = startDate.AddDays(1 - startDate.Day);
            }
            else
            {
                startDate = startDate.AddDays(15 - startDate.Day);
                totaldays = DateTime.DaysInMonth(startDate.Year, startDate.Month) - totaldays;
            }
            Receipts receipts = new Receipts()
            {
                StartDate = startDate,
                Items = Enumerable.Repeat("", totaldays).ToList(),
                Amounts = Enumerable.Repeat(0, totaldays).ToList(),
                Paths = Enumerable.Repeat("", totaldays).ToList(),
            };
            Invoice invoice = _invoiceRepository.GetInvoiceByPhysician(aspNetUserId, startDate);
            if (invoice != null)
            {
                foreach (Reimbursement reimbursement in invoice.Reimbursements)
                {
                    receipts.Items[reimbursement.Date.Day - 1] = reimbursement.Item;
                    receipts.Amounts[reimbursement.Date.Day - 1] = reimbursement.Amount;
                    receipts.Paths[reimbursement.Date.Day - 1] = GetFile(aspNetUserId, reimbursement.Date.Day, reimbursement.Date.Month);
                }
            }
            return receipts;
        }

        public async Task<bool> SaveInvoice(CreateInvoice model, int aspNetUserId, bool isFinalize)
        {
            Invoice invoice = _invoiceRepository.GetInvoiceByPhysician(aspNetUserId, model.StartDate);
            if (invoice == null)
            {
                return await CreateInvoice(model, aspNetUserId);
            }
            else
            {
                Physician physician = _userRepository.GetPhysicianByAspNetUserId(aspNetUserId);
                List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>();
                List<Reimbursement> reimbursements = new List<Reimbursement>();
                int fileIndex = 0;
                for (int i = 0; i < model.TotalHours.Count; i++)
                {
                    InvoiceDetail invoiceDetail = invoice.InvoiceDetails.First(x => x.Date.Day == model.StartDate.AddDays(i).Day);
                    invoiceDetail.InvoiceId = invoice.InvoiceId;
                    invoiceDetail.Date = model.StartDate.AddDays(i);
                    invoiceDetail.TotalHours = model.TotalHours[i];
                    invoiceDetail.IsHoliday = model.IsHoliday != null ? model.IsHoliday.Contains(model.StartDate.AddDays(i).Day) : false;
                    invoiceDetail.NumberOfHouseCall = model.NoOfHouseCall[i];
                    invoiceDetail.NumberOfPhoneConsults = model.NoOfPhoneConsults[i];
                    invoiceDetail.ModifyDate = DateTime.Now;
                    invoiceDetail.ModifyBy = aspNetUserId;
                    invoiceDetails.Add(invoiceDetail);
                    if (model.Receipts != null && model.Receipts.Items[i] != null)
                    {
                        Reimbursement reimbursement = invoice.Reimbursements.FirstOrDefault(x => x.Date.Day == model.StartDate.AddDays(i).Day);
                        if (reimbursement != null)
                        {
                            reimbursement.Item = model.Receipts.Items[i];
                            reimbursement.Amount = model.Receipts.Amounts[i];
                            reimbursement.Date = model.StartDate.AddDays(i);
                            reimbursement.ModifyDate = DateTime.Now;
                            reimbursement.ModifyBy = aspNetUserId;
                            reimbursements.Add(reimbursement);
                        }
                        else
                        {
                            reimbursements.Add(new Reimbursement()
                            {
                                InvoiceId = invoice.InvoiceId,
                                Item = model.Receipts.Items[i],
                                Amount = model.Receipts.Amounts[i],
                                RequestWiseFileId = await FileUpload(model.Receipts.Bill[fileIndex], i + 1, model.StartDate.Month, physician),
                                Date = model.StartDate.AddDays(i),
                                PhysicianId = physician.PhysicianId,
                                CreatedDate = DateTime.Now,
                                CreatedBy = aspNetUserId,
                            });
                        }
                    }
                }
                if (await _invoiceRepository.UpdateInvoiceDetails(invoiceDetails))
                {
                    if(reimbursements.Count > 0)
                    {
                        await _invoiceRepository.UpdateReimbursement(reimbursements);
                    }
                    if (isFinalize)
                    {
                        invoice.Status = true;
                        return await _invoiceRepository.UpdateInvoice(invoice);
                    }
                    return true;
                }
            }
            return true;
        }

        private async Task<bool> CreateInvoice(CreateInvoice model, int aspNetUserId)
        {
            Physician physician = _userRepository.GetPhysicianByAspNetUserId(aspNetUserId);
            Invoice invoice = new Invoice()
            {
                PhysicianId = physician.PhysicianId,
                StartDate = model.StartDate,
                EndDate = model.StartDate.AddDays(14),
                Status = false,
                CreatedDate = DateTime.Now,
                CreatedBy = aspNetUserId,
            };
            if (await _invoiceRepository.AddInvoice(invoice))
            {
                List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>();
                List<Reimbursement> reimbursements = new List<Reimbursement>();
                int fileIndex = 0;
                for (int i = 0; i < model.TotalHours.Count; i++)
                {
                    invoiceDetails.Add(new InvoiceDetail()
                    {
                        InvoiceId = invoice.InvoiceId,
                        Date = model.StartDate.AddDays(i),
                        TotalHours = model.TotalHours[i],
                        IsHoliday = model.IsHoliday != null ? model.IsHoliday.Contains(model.StartDate.AddDays(i).Day) : false,
                        NumberOfHouseCall = model.NoOfHouseCall[i],
                        NumberOfPhoneConsults = model.NoOfPhoneConsults[i],
                        CreatedDate = DateTime.Now,
                        CreatedBy = aspNetUserId,
                    });
                    if (model.Receipts != null && model.Receipts.Items[i] != null)
                    {
                        reimbursements.Add(new Reimbursement()
                        {
                            InvoiceId = invoice.InvoiceId,
                            Item = model.Receipts.Items[i],
                            Amount = model.Receipts.Amounts[i],
                            RequestWiseFileId = await FileUpload(model.Receipts.Bill[fileIndex], i + 1, model.StartDate.Month, physician),
                            Date = model.StartDate.AddDays(i),
                            PhysicianId = physician.PhysicianId,
                            CreatedDate = DateTime.Now,
                            CreatedBy = aspNetUserId,
                        });
                        fileIndex++;
                    }
                }
                if (await _invoiceRepository.AddInvoiceDetails(invoiceDetails))
                {
                    return await _invoiceRepository.AddReimbursement(reimbursements);
                }
            }
            return false;
        }

        private string GetFile(int aspnetUserId, int day, int month)
        {
            String path = Path.Combine($"/Files//Invoice/{aspnetUserId}/{month}/{day}");
            String _path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/Files//Invoice/{aspnetUserId}/{month}/{day}");
            FileInfo[] Files = new DirectoryInfo(_path).GetFiles().OrderBy(p => p.LastWriteTime).ToArray();
            return Path.Combine(path, Files[^1].Name);
        }

        private async Task<int> FileUpload(IFormFile file, int day, int month, Physician physician)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/Files//Invoice/{physician.AspNetUserId}/{month}/{day}");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            };
            FileInfo fileInfo = new FileInfo(file.FileName);
            string fileName = fileInfo.Name;
            string fileNameWithPath = Path.Combine(path, fileName);
            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            RequestWiseFile requestWiseFile = new()
            {
                PhysicianId = physician.PhysicianId,
                FileName = fileName,
                CreatedDate = DateTime.Now,
                Uploder = $"{physician.FirstName} {physician.LastName}",
                IsDeleted = new BitArray(1, false),
            };
            await _requestWiseFileRepository.AddFile(requestWiseFile);
            return requestWiseFile.RequestWiseFileId;
        }
    }
}
