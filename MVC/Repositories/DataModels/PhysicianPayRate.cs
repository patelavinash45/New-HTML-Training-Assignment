using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Repositories.DataModels;

[Keyless]
[Table("PhysicianPayRate")]
public partial class PhysicianPayRate
{
    public int PayRateId { get; set; }

    public int PhysicianId { get; set; }

    public double? NightShiftWeekend { get; set; }

    public double? Shift { get; set; }

    public double? HouseCallNightWeekend { get; set; }

    public double? PhoneConsults { get; set; }

    public double? PhoneConsultsNightWeekend { get; set; }

    public double? BatchTesting { get; set; }

    public double? HouseCall { get; set; }
}
