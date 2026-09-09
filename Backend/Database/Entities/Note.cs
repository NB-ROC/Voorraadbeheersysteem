using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Database.Entities;

public class Note
{
    // Navigation properties

    [ForeignKey(nameof(WriterId))] public User Writer = null!;

    public ulong Id { get; set; }

    public ulong WriterId { get; set; }

    [MinLength(1)] [MaxLength(512)] public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}