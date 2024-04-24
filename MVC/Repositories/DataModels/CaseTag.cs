using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.DataModels;

[Table("CaseTag")]
public partial class CaseTag
{
    [Key]
    public int CaseTagId { get; set; }

    [StringLength(50)]
    public string Reason { get; set; } = null!;

    [InverseProperty("CaseTag")]
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
