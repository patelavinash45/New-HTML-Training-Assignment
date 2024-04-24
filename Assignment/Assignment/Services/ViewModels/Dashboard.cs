using Repositories.DataModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels
{
    public class Dashboard
    {
        public int PageNo { get; set; }

        public int CurrentPageSize { get; set; }

        public int TotalStudents { get; set; }

        public bool IsNext { get; set; }

        public bool IsPrevious { get; set; }

        public List<DashboardTableData> StudentsRecords { get; set; }

        public StudentData StudentData { get; set; }
    }

    public class DashboardTableData
    {
        public int Id { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public int? Courseid { get; set; }

        public short? Age { get; set; }

        public string? Email { get; set; }

        public short? Gender { get; set; }

        [StringLength(50)]
        public string? Course { get; set; }

        public string? Grade { get; set; }

    }

    public class StudentData
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        [Required]
        public short? Gender { get; set; }

        [Required]
        public DateTime? DateofBirth { get; set; }

        public string Date { get; set; }

        public string Grade { get; set; }

        public string Course { get; set; }

        public List<string> CourseList { get; set; }
    }
}
