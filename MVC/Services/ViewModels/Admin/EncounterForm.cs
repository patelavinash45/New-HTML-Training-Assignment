using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.Admin
{
    public class EncounterForm
    {
        public bool IsFinalize { get; set; } = false;

        public bool IsAdmin { get; set; } = true;

        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The FirstName is not valid")]
        [Required(ErrorMessage = "The FirstName field is required.")]
        public string? FirstName { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The LastName is not valid")]
        [Required(ErrorMessage = "The LastName field is required.")]
        public string? LastName { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Location field is required.")]
        public string? Location { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "The Email is not valid")]
        [Required(ErrorMessage = "The Email field is required.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "The BirthDate field is required.")]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "The Date field is required.")]
        public DateTime? Date { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^(?:\+?91)?\s*([1-9]\d{4})\s*(\d{5})$", ErrorMessage = "The Mobile is not valid")]
        [Required(ErrorMessage = "The Mobile field is required.")]
        public string? Mobile { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The HistoryOfIllness field is required.")]
        public string? HistoryOfIllness { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The MedicalHistory field is required.")]
        public string? MedicalHistory { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Medications field is required.")]
        public string? Medications { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Allergies field is required.")]
        public string? Allergies { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "The Temp field is required.")]
        public string? Temp { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "The HeartRate field is required.")]
        public string? HeartRate { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "The RespiratoryRate field is required.")]
        public string? RespiratoryRate { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "The BloodPressure field is required.")]
        public string? BloodPressure1 { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "The BloodPressure field is required.")]
        public string? BloodPressure2 { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "The O2 field is required.")]
        public string? O2 { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Pain field is required.")]
        public string? Pain { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Heent field is required.")]
        public string? Heent { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The CV field is required.")]
        public string? CV { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Chest field is required.")]
        public string? Chest { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The ABD field is required.")]
        public string? ABD { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Extra field is required.")]
        public string? Extra { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Skin field is required.")]
        public string? Skin { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Neuro field is required.")]
        public string? Neuro { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Other field is required.")]
        public string? Other { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Diagnosis field is required.")]
        public string? Diagnosis { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The TreatmentPlan field is required.")]
        public string? TreatmentPlan { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Dispensed field is required.")]
        public string? Dispensed { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The Procedures field is required.")]
        public string? Procedures { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "The FollowUp field is required.")]
        public string? FollowUp { get; set; }
    }

}
