using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.DataModels;

[Table("EmailLog")]
public partial class EmailLog
{
    [Column(TypeName = "character varying")]
    public string? EmailTemplate { get; set; }

    [StringLength(200)]
    public string SubjectName { get; set; } = null!;

    [Column("EmailID")]
    [StringLength(200)]
    public string EmailId { get; set; } = null!;

    [StringLength(200)]
    public string? ConfirmationNumber { get; set; }

    [Column(TypeName = "character varying")]
    public string? FilePath { get; set; }

    public int? RoleId { get; set; }

    public int? RequestId { get; set; }

    public int? AdminId { get; set; }

    public int? PhysicianId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreateDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime SentDate { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsEmailSent { get; set; }

    public int? SentTries { get; set; }

    public int? Action { get; set; }

    [Column(TypeName = "character varying")]
    public string? Name { get; set; }

    [Key]
    public int EmailLogId { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("EmailLogs")]
    public virtual Role? Role { get; set; }
}
