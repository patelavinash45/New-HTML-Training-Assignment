using Microsoft.AspNetCore.Http;

namespace Services.ViewModels.Physician
{
    public class InvoicePage
    {
        public Dictionary<int,string>? Physicians { get; set; }

        public Dictionary<DateTime, DateTime> Dates { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Status { get; set; }

        public bool? IsApprove { get; set; }

        public int? InvoiceId { get; set; }
    }

    public class CreateInvoice
    {
        public DateOnly StartDate { get; set; }

        public Dictionary<int,double> ShiftHours { get; set; }

        public List<double> TotalHours { get; set; } = new List<double>();

        public List<int> IsHoliday { get; set; } = new List<int>();

        public List<int> NoOfHouseCall { get; set; } = new List<int>();

        public List<int> NoOfPhoneConsults { get; set; } = new List<int>();

        public Receipts Receipts { get; set; }
    }

    public class Receipts
    {
        public DateOnly StartDate { get; set; }

        public List<string> Items { get; set; } = new List<string>();

        public List<int> Amounts { get; set; } = new List<int>();

        public List<IFormFile> Bill { get; set; }

        public List<string> Paths { get; set; } = new List<string>();
    }
}
