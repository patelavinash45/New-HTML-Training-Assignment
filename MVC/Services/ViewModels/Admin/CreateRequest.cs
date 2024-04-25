using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.Admin
{
    public class CreateRequest
    {
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

        [StringLength(20)]
        [RegularExpression(@"^(?:\+?91)?\s*([1-9]\d{4})\s*(\d{5})$", ErrorMessage = "The Mobile is not valid")]
        [Required(ErrorMessage = "The Mobile field is required.")]
        public string? Mobile { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The Street field is required.")]
        public string? Street { get; set; }

        public string? Password { get; set; }

        [CompareAttribute("Password", ErrorMessage = "The Password doesn't match.")]
        public string? ConformPassword { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The City field is required.")]
        public string? City { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The State field is required.")]
        public string? State { get; set; }

        [StringLength(10)]
        [Required(ErrorMessage = "The ZipCode field is required.")]
        public string? ZipCode { get; set; }

        public int Region { get; set; }

        public string? Room { get; set; }

        [StringLength(500)]
        public string? Symptoms { get; set; }

        [Required(ErrorMessage = "The BirthDate field is required.")]
        public DateTime? BirthDate { get; set; }
    }
}
