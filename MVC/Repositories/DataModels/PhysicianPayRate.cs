using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Repositories.DataModels;

[Table("PhysicianPayRate")]
public partial class PhysicianPayRate
{
    [Key]
    public int PayRateId { get; set; }

    public int PhysicianId { get; set; }

    public double? NightShiftWeekend { get; set; }

    public double? Shift { get; set; }

    public double? HouseCallNightWeekend { get; set; }

    public double? PhoneConsults { get; set; }

    public double? PhoneConsultsNightWeekend { get; set; }

    public double? BatchTesting { get; set; }

    public double? HouseCall { get; set; }

    public int CreatedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    public int? ModifyBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifyDate { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("PhysicianPayRates")]
    public virtual Physician Physician { get; set; } = null!;
}
