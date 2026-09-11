using System.ComponentModel.DataAnnotations;
using Backend.Database.Entities.Junctions;

namespace Backend.Database.Entities;

public class Note
{
    public ulong Id { get; set; }

    public ulong? WriterId { get; set; }

    [MinLength(1)] [MaxLength(512)] public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties

    public User? Writer { get; set; } = null!;
    
    public ICollection<UserNote> UserNotes { get; set; } = [];
    public ICollection<ProductNote> ProductNotes { get; set; } = [];
}