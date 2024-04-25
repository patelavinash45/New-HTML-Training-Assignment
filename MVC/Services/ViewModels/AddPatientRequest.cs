using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels
{
    public class AddPatientRequest
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

        [StringLength(100)]
        public string? Password { get; set; }

        [StringLength(100)]
        [CompareAttribute("Password", ErrorMessage = "The Password doesn't match.")]
        public string? ConformPassword { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^(?:\+?91)?\s*([1-9]\d{4})\s*(\d{5})$", ErrorMessage = "The Mobile is not valid")]
        [Required(ErrorMessage = "The Mobile field is required.")]
        public string? Mobile { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The Street field is required.")]
        public string? Street { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The City field is required.")]
        public string? City { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The State field is required.")]
        public string? State { get; set; }

        [StringLength(10)]
        [Required(ErrorMessage = "The ZipCode field is required.")]
        public string? ZipCode { get; set; }

        [Required(ErrorMessage = "The BirthDate field is required.")]
        public DateTime BirthDate { get; set; }

        public int Region { get; set; }

        public IFormFile? File { get; set; }

        [StringLength(100)]
        public string? House { get; set; }

        [StringLength(500)]
        public string? Symptoms { get; set; }
    }

    public class AddFamilyRequest
    {
        [StringLength(100)]
        [Display(Name = "FirstName")]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The FirstName is not valid")]
        [Required(ErrorMessage = "The FirstName field is required.")]
        public string? FamilyFriendFirstName { get; set; }

        [StringLength(100)]
        [Display(Name = "LastName")]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The LastName is not valid")]
        [Required(ErrorMessage = "The LastName field is required.")]
        public string? FamilyFriendLastName { get; set; }

        [StringLength(50)]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "The Email field is required.")]
        public string? FamilyFriendEmail { get; set; }

        [StringLength(20)]
        [Display(Name = "Mobile")]
        [RegularExpression(@"^(?:\+?91)?\s*([1-9]\d{4})\s*(\d{5})$", ErrorMessage = "The Mobile is not valid")]
        [Required(ErrorMessage = "The Mobile field is required.")]
        public string? FamilyFriendMobile { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The Relation field is required.")]
        public string? Relation { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The FirstName is not valid")]
        [Required(ErrorMessage = "The FirstName field is required.")]
        public string? FirstName { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The LastName is not valid")]
        [Required(ErrorMessage = "The LastName field is required.")]
        public string? LastName { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "The Email field is required.")]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? Password { get; set; }

        [StringLength(100)]
        [CompareAttribute("Password", ErrorMessage = "The Password doesn't match.")]
        public string? ConformPassword { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^(?:\+?91)?\s*([1-9]\d{4})\s*(\d{5})$", ErrorMessage = "The Mobile is not valid")]
        [Required(ErrorMessage = "The Mobile field is required.")]
        public string? Mobile { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The Street field is required.")]
        public string? Street { get; set; }

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

        [StringLength(500)]
        public string? Symptoms { get; set; }

        [StringLength(100)]
        public string? House { get; set; }

        [Required(ErrorMessage = "The BirthDate field is required.")]
        public DateTime? BirthDate { get; set; }

        public IFormFile? File { get; set; }
    }

    public class AddConciergeRequest
    {
        [StringLength(100)]
        [Display(Name = "FirstName")]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The FirstName is not valid")]
        [Required(ErrorMessage = "The FirstName field is required.")]
        public string? ConciergeFirstName { get; set; }

        [StringLength(100)]
        [Display(Name = "LastName")]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The LastName is not valid")]
        [Required(ErrorMessage = "The LastName field is required.")]
        public string? ConciergeLastName { get; set; }

        [StringLength(50)]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "The Email is not valid")]
        [Required(ErrorMessage = "The Email field is required.")]
        public string? ConciergeEmail { get; set; }

        [StringLength(20)]
        [Display(Name = "Mobile")]
        [RegularExpression(@"^(?:\+?91)?\s*([1-9]\d{4})\s*(\d{5})$", ErrorMessage = "The Mobile is not valid")]
        [Required(ErrorMessage = "The Mobile field is required.")]
        public string? ConciergeMobile { get; set; }

        [StringLength(100)]
        [Display(Name = "PropertyName")]
        [Required(ErrorMessage = "The PropertyName field is required.")]
        public string? ConciergePropertyName { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The Street field is required.")]
        public string? ConciergeStreet { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The City field is required.")]
        public string? ConciergeCity { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The State field is required.")]
        public string? ConciergeState { get; set; }

        [StringLength(10)]
        [Required(ErrorMessage = "The ZipCode field is required.")]
        public string? ConciergeZipCode { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The FirstName is not valid")]
        [Required(ErrorMessage = "The FirstName field is required.")]
        public string? FirstName { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The LastName is not valid")]
        [Required(ErrorMessage = "The LastName field is required.")]
        public string? LastName { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "The Email field is required.")]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? Password { get; set; }

        [StringLength(100)]
        [CompareAttribute("Password", ErrorMessage = "The Password doesn't match.")]
        public string? ConformPassword { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^(?:\+?91)?\s*([1-9]\d{4})\s*(\d{5})$", ErrorMessage = "The Mobile is not valid")]
        [Required(ErrorMessage = "The Mobile field is required.")]
        public string? Mobile { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The Street field is required.")]
        public string? Street { get; set; }

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

        [StringLength(500)]
        public string? Symptoms { get; set; }

        [StringLength(100)]
        public string? House { get; set; }

        [Required(ErrorMessage = "The BirthDate field is required.")]
        public DateTime? BirthDate { get; set; }

        public IFormFile? File { get; set; }

    }

    public class AddBusinessRequest
    {
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The FirstName is not valid")]
        [Required(ErrorMessage = "The FirstName field is required.")]
        public string? BusinessFirstName { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The LastName is not valid")]
        [Required(ErrorMessage = "The LastName field is required.")]
        public string? BusinessLastName { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "The Email is not valid")]
        [Required(ErrorMessage = "The Email field is required.")]
        public string? BusinessEmail { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^(?:\+?91)?\s*([1-9]\d{4})\s*(\d{5})$", ErrorMessage = "The Mobile is not valid")]
        [Required(ErrorMessage = "The Mobile field is required.")]
        public string? BusinessMobile { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The Business field is required.")]
        public string? Business { get; set; }

        [StringLength(100)]
        public string? CaseNumber { get; set; }

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

        [StringLength(100)]
        public string? Password { get; set; }

        [StringLength(100)]
        [CompareAttribute("Password", ErrorMessage = "The Password doesn't match.")]
        public string? ConformPassword { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^(?:\+?91)?\s*([1-9]\d{4})\s*(\d{5})$", ErrorMessage = "The Mobile is not valid")]
        [Required(ErrorMessage = "The Mobile field is required.")]
        public string? Mobile { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The Street field is required.")]
        public string? Street { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The City field is required.")]
        public string? City { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The State field is required.")]
        public string? State { get; set; }

        public int Region { get; set; }

        [StringLength(10)]
        [Required(ErrorMessage = "The ZipCode field is required.")]
        public string? ZipCode { get; set; }

        [StringLength(500)]
        public string? Symptoms { get; set; }

        [StringLength(100)]
        public string? House { get; set; }

        [Required(ErrorMessage = "The BirthDate field is required.")]
        public DateTime? BirthDate { get; set; }

        public IFormFile? File { get; set; }
    }

    public class AddRequestByPatient
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
        [Required(ErrorMessage = "The Email field is required.")]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? Password { get; set; }

        [StringLength(100)]
        [CompareAttribute("Password", ErrorMessage = "The Password doesn't match.")]
        public string? ConformPassword { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^(?:\+?91)?\s*([1-9]\d{4})\s*(\d{5})$", ErrorMessage = "The Mobile is not valid")]
        [Required(ErrorMessage = "The Mobile field is required.")]
        public string? Mobile { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The Street field is required.")]
        public string? Street { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The City field is required.")]
        public string? City { get; set; }

        public int Region { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The State field is required.")]
        public string? State { get; set; }

        [StringLength(10)]
        [Required(ErrorMessage = "The ZipCode field is required.")]
        public string? ZipCode { get; set; }

        [Required(ErrorMessage = "The Relation field is required.")]
        public string? Relation { get; set; }

        [Required(ErrorMessage = "The BirthDate field is required.")]
        public DateTime BirthDate { get; set; }

        public IFormFile? File { get; set; }

        [StringLength(100)]
        public string? House { get; set; }

        [StringLength(500)]
        public string? Symptoms { get; set; }
    }
}
