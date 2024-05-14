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

    public int? ReceiverId { get; set; }

    [Column(TypeName = "character varying")]
    public string? Message { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? Time { get; set; }
}
