using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Repositories.DataModels;

[Table("Invoice")]
public partial class Invoice
{
    [Key]
    public int InvoiceId { get; set; }

    public int PhysicianId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool Status { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifyBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifyDate { get; set; }

    public bool? Approved { get; set; }

    [InverseProperty("Invoice")]
    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    [ForeignKey("PhysicianId")]
    [InverseProperty("Invoices")]
    public virtual Physician Physician { get; set; } = null!;

    [InverseProperty("Invoice")]
    public virtual ICollection<Reimbursement> Reimbursements { get; set; } = new List<Reimbursement>();
}
