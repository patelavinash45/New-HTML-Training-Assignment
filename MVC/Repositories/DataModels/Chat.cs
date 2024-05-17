using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Repositories.DataModels;

[Table("Chat")]
public partial class Chat
{
    [Key]
    public int ChatId { get; set; }

    public int? SenderId { get; set; }

    public int? RequestId { get; set; }

    [Column(TypeName = "character varying")]
    public string? Message { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? Time { get; set; }

    public int? Type { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("Chats")]
    public virtual Request? Request { get; set; }
}
