using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.DataModels;

[Table("Reimbursement")]
public partial class Reimbursement
{
    [Key]
    public int Id { get; set; }

    public int? InvoiceId { get; set; }

    public int? InvoiceDetailsId { get; set; }

    [StringLength(100)]
    public string? Item { get; set; }

    public int? Amount { get; set; }

    public int? RequestWiseFileId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? Date { get; set; }

    [ForeignKey("InvoiceId")]
    [InverseProperty("Reimbursements")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("InvoiceDetailsId")]
    [InverseProperty("Reimbursements")]
    public virtual InvoiceDetail? InvoiceDetails { get; set; }

    [ForeignKey("RequestWiseFileId")]
    [InverseProperty("Reimbursements")]
    public virtual RequestWiseFile? RequestWiseFile { get; set; }
}
