using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.Admin
{
    public class AdminCreaateAndProfile
    {
        [Required(ErrorMessage = "The UserName field is required.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "The Password field is required.")]
        public string? Password { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The FirstName is not valid")]
        [Required(ErrorMessage = "The FirstName field is required.")]
        public string? FirstName { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The LastName is not valid")]
        [Required(ErrorMessage = "The LastName field is required.")]
        public string? LastName { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "The Email is not valid")]
        [Required(ErrorMessage = "The Email field is required.")]
        public string? Email { get; set; }

        [StringLength(50)]
        [CompareAttribute("Email", ErrorMessage = "Email doesn't match.")]
        [Required(ErrorMessage = "The ConfirmEmail field is required.")]
        public string? ConfirmEmail { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "The Phone field is required.")]
        public string? Phone { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^(?:\+?91)?\s*([1-9]\d{4})\s*(\d{5})$", ErrorMessage = "The Mobile is not valid")]
        [Required(ErrorMessage = "The Mobile field is required.")]
        public string? Mobile { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Address field is required.")]
        public string? Address1 { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Address field is required.")]
        public string? Address2 { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The City field is required.")]
        public string? City { get; set; }

        [StringLength(10)]
        [Required(ErrorMessage = "The ZipCode field is required.")]
        public string? ZipCode { get; set; }

        [Required(ErrorMessage = "The State field is required.")]
        public string? SelectedRegion { get; set; } 

        [Required(ErrorMessage = "The Region field is required.")]
        public List<string>? SelectedRegions { get; set; }

        public Dictionary<int, string>? Regions { get; set; }

        public Dictionary<int, bool>? AdminRegions { get; set; }

        public short? Status { get; set; }

        [Required(ErrorMessage = "The Role field is required.")]
        public int SelectedRole { get; set; }

        public Dictionary<int, string>? Roles { get; set; }

    }
}
