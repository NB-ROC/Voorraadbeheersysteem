using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Database.Entities;

public class Log
{
    public ulong Id { get; set; }

    public ulong InvokerId { get; set; }

    public ulong RelatedId { get; set; }

    [MinLength(1)] [MaxLength(255)] public string Action { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties

    [ForeignKey(nameof(InvokerId))] public User Invoker { get; set; } = null!;

    [ForeignKey(nameof(RelatedId))] public User Related { get; set; } = null!;
}