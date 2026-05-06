using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class Category
{
    public const int CategoryLength = 32;

    public int Id { get; set; }

    [MaxLength(CategoryLength)] public string Name { get; set; } = null!;
}