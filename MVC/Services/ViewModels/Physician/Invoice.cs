using Microsoft.AspNetCore.Http;

namespace Services.ViewModels.Physician
{
    public class InvoicePage
    {
        public DateTime Date { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string Status { get; set; }
    }

    public class CreateInvoice
    {
        public List<int> ShiftHours { get; set; }

        public List<int> TotalHours { get; set; }

        public List<bool> IsHoliday { get; set; }

        public List<int> NoOfHouseCall { get; set; }

        public List<int> NoOfPhoneConsults { get; set; }

        public List<string> Items { get; set; }

        public List<int> Amounts { get; set; }

        public List<IFormFile> Biil { get; set; }
    }
}
