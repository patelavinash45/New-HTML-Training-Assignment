using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.DataModels;

[Table("Invoice")]
public partial class Invoice
{
    [Key]
    public int InvoiceId { get; set; }

    public int? PhysicianId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? EndDate { get; set; }

    public bool? Status { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [InverseProperty("Invoice")]
    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    [ForeignKey("PhysicianId")]
    [InverseProperty("Invoices")]
    public virtual Physician? Physician { get; set; }

    [InverseProperty("Invoice")]
    public virtual ICollection<Reimbursement> Reimbursements { get; set; } = new List<Reimbursement>();
}
