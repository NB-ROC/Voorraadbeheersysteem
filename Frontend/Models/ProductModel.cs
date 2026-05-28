using System.ComponentModel.DataAnnotations;

namespace Frontend.Models;

public class ProductModel
{
    public const int NameLength = 64;
    public const int CategoryLength = 45;
    public const int DescriptionLength = 128;
    public const int ImageLength = 16;

    public int Id { get; set; }

    [MaxLength(CategoryLength)] public string Name { get; set; } = null!;

    public CategoryModel Category { get; set; } = null!;

    [MaxLength(DescriptionLength)] public string Description { get; set; } = null!;

    [MaxLength(ImageLength)] public string Image { get; set; } = null!;

    public int Status { get; set; }
}