using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Repositories.DataModels;

[Table("student")]
public partial class Student
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("firstname")]
    [StringLength(50)]
    public string? Firstname { get; set; }

    [Column("lastname")]
    [StringLength(50)]
    public string? Lastname { get; set; }

    [Column("courseid")]
    public int? Courseid { get; set; }

    [Column("age")]
    public short? Age { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("course")]
    [StringLength(50)]
    public string? Course { get; set; }

    [Column("grade")]
    [StringLength(20)]
    public string? Grade { get; set; }

    public bool? IsDelete { get; set; }

    [Column("gender")]
    public short? Gender { get; set; }

    [Column("birthDate", TypeName = "timestamp without time zone")]
    public DateTime? BirthDate { get; set; }

    [ForeignKey("Courseid")]
    [InverseProperty("Students")]
    public virtual Course? CourseNavigation { get; set; }
}
