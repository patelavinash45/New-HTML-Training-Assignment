using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.DataModels;

public partial class InvoiceDetail
{
    [Key]
    public int InvoiceDetailsId { get; set; }

    public int? InvoiceId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? Date { get; set; }

    public int? TotalHours { get; set; }

    public bool? IsHoliday { get; set; }

    public int? NumberOfHouseCall { get; set; }

    public int? NumberOfPhoneConsults { get; set; }

    [ForeignKey("InvoiceId")]
    [InverseProperty("InvoiceDetails")]
    public virtual Invoice? Invoice { get; set; }

    [InverseProperty("InvoiceDetails")]
    public virtual ICollection<Reimbursement> Reimbursements { get; set; } = new List<Reimbursement>();
}
