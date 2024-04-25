using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.Admin
{
    public class Partners
    {
        public Dictionary<int,String> ProfessionList { get; set; }

        public List<PartnersTableData> PartnersTableDatas {  get; set; }
    }

    public class PartnersTableData
    {
        public int VenderId { get; set; }

        public string? Profession { get; set; }

        public string? BusinessName { get; set; }

        public string? Email { get; set; }

        public string? FaxNumber { get; set; }

        public string? PhoneNumber { get; set; }

        public string? BusinessContact { get; set; }
    }

    public class BusinessProfile
    {
        public string AspAction { get; set; } = "CreateBusiness";

        public bool IsUpdate { get; set; } = false;

        public Dictionary<int, String>? ProfessionList { get; set; }

        public Dictionary<int, String>? RegionList { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The VendorName field is required.")]
        public string? VendorName { get; set; }

        public int Profession { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "The FaxNumber field is required.")]
        public string? FaxNumber { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The City field is required.")]
        public string? City { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "The State field is required.")]
        public string? State { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "The Street field is required.")]
        public string? Street { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "The Zip field is required.")]
        public string? Zip { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^(?:\+?91)?\s*([1-9]\d{4})\s*(\d{5})$", ErrorMessage = "The PhoneNumber is not valid")]
        [Required(ErrorMessage = "The PhoneNumber field is required.")]
        public string? PhoneNumber { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "The Email is not valid")]
        [Required(ErrorMessage = "The Email field is required.")]
        public string? Email { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^(?:\+?91)?\s*([1-9]\d{4})\s*(\d{5})$", ErrorMessage = "The BusinessContact is not valid")]
        [Required(ErrorMessage = "The BusinessContact field is required.")]
        public string? BusinessContact { get; set; }
    }
}
