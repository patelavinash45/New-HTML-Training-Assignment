using Microsoft.AspNetCore.Http;

namespace Services.ViewModels.Physician
{
    public class InvoicePage
    {
        public DateTime Date { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string Status { get; set; }

        public CreateInvoice CreateInvoice { get; set; }
    }

    public class CreateInvoice
    {
        public DateTime StartDate { get; set; }

        public Dictionary<int,double> ShiftHours { get; set; }

        public List<int> TotalHours { get; set; }

        public List<int> IsHoliday { get; set; }

        public List<int> NoOfHouseCall { get; set; }

        public List<int> NoOfPhoneConsults { get; set; }

        public List<string> Items { get; set; }

        public List<int> Amounts { get; set; }

        public List<IFormFile> Biil { get; set; }
    }
}
