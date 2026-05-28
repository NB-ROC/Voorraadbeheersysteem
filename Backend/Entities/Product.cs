using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class Product
{
    public const int NameLength = 64;
    public const int DescriptionLength = 128;
    public const int ImageLength = 16;

    public int Id { get; set; }

    [MaxLength(NameLength)] public string Name { get; set; } = null!;
    [MaxLength(DescriptionLength)] public string Description { get; set; } = null!;
    [MaxLength(ImageLength)] public string Image { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}