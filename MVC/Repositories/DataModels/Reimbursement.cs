using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Repositories.DataModels;

[Table("Reimbursement")]
public partial class Reimbursement
{
    [Key]
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public int InvoiceDetailsId { get; set; }

    [StringLength(100)]
    public string Item { get; set; } = null!;

    public int Amount { get; set; }

    public int RequestWiseFileId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime Date { get; set; }

    public int PhysicianId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifyDate { get; set; }

    public int? ModifyBy { get; set; }

    [ForeignKey("InvoiceId")]
    [InverseProperty("Reimbursements")]
    public virtual Invoice Invoice { get; set; } = null!;

    [ForeignKey("InvoiceDetailsId")]
    [InverseProperty("Reimbursements")]
    public virtual InvoiceDetail InvoiceDetails { get; set; } = null!;

    [ForeignKey("PhysicianId")]
    [InverseProperty("Reimbursements")]
    public virtual Physician Physician { get; set; } = null!;

    [ForeignKey("RequestWiseFileId")]
    [InverseProperty("Reimbursements")]
    public virtual RequestWiseFile RequestWiseFile { get; set; } = null!;
}
