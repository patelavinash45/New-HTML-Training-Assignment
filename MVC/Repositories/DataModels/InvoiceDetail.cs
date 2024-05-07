using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Repositories.DataModels;

public partial class InvoiceDetail
{
    [Key]
    public int InvoiceDetailsId { get; set; }

    public int InvoiceId { get; set; }

    public DateOnly Date { get; set; }

    public double TotalHours { get; set; }

    public bool IsHoliday { get; set; }

    public int NumberOfHouseCall { get; set; }

    public int NumberOfPhoneConsults { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifyDate { get; set; }

    public int? ModifyBy { get; set; }

    [ForeignKey("InvoiceId")]
    [InverseProperty("InvoiceDetails")]
    public virtual Invoice Invoice { get; set; } = null!;

    [InverseProperty("InvoiceDetails")]
    public virtual ICollection<Reimbursement> Reimbursements { get; set; } = new List<Reimbursement>();
}
