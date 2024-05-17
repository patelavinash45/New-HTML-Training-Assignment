using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels
{
    public class Dashboard
    {
        public int? RequestId { get; set; }

        public string? StrMonth { get; set; }

        public int? IntYear { get; set; }

        public int? IntDate { get; set; }

        public int? Status { get; set; }

        public int Document { get; set; }

        public bool IsAssigned { get; set; }
    }
}
