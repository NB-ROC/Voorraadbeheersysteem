using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class Product
{
    public const int NameLength = 64;
    public const int CategoryLength = 45;
    public const int DescriptionLength = 45;
    public const int ImageLength = 16;

    public int Id { get; set; }

    [MaxLength(NameLength)] public string Name { get; set; } = null!;

    [MaxLength(CategoryLength)] public string Category { get; set; } = null!;

    [MaxLength(DescriptionLength)] public string Description { get; set; } = null!;

    [MaxLength(ImageLength)] public string Image { get; set; } = null!;

    public int Amount { get; set; }

    public ICollection<LoanProduct> LoanProducts { get; set; } = new List<LoanProduct>();
}