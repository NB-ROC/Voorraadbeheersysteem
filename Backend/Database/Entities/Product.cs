using System.ComponentModel.DataAnnotations;
using Backend.Database.Entities.Junctions;

namespace Backend.Database.Entities;

public class Product
{
    public ulong Id { get; set; }

    [MinLength(1)] [MaxLength(32)] public string Name { get; set; } = string.Empty;

    [MinLength(1)] [MaxLength(512)] public string Description { get; set; } = string.Empty;

    [Required] public int Amount { get; set; }

    [MinLength(1)] [MaxLength(64)] public string? ImageId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties

    public ICollection<LoanProduct> LoanProducts { get; set; } = [];
    public ICollection<ProductRole> ProductRoles { get; set; } = [];
    public ICollection<ProductNote> ProductNotes { get; set; } = [];
}