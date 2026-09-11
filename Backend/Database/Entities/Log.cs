using System.ComponentModel.DataAnnotations;

namespace Backend.Database.Entities;

public class Log
{
    public ulong Id { get; set; }

    public ulong? InvokerId { get; set; }

    public ulong? TargetId { get; set; }

    [MinLength(1)] [MaxLength(255)] public string Action { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties

    public User? Invoker { get; set; }

    public User? Target { get; set; }
}