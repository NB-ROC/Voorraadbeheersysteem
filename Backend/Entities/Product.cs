using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class Product
{
    public const int NameLength = 64;
    public const int CategoryLength = 45;
    public const int DescriptionLength = 128;
    public const int ImageLength = 16;

    public int Id { get; set; }

    [MaxLength(NameLength)] public string Name { get; set; } = null!;
    [MaxLength(DescriptionLength)] public string Description { get; set; } = null!;
    [MaxLength(ImageLength)] public string Image { get; set; } = null!;
    
    
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    public ProductStatus Status { get; set; }

    public int? RestrictedRoleId { get; set; }
    public Role? RestrictedRole { get; set; } 
    
    public DateTime? PurchaseDate { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<LoanProduct> LoanProducts { get; set; } = new List<LoanProduct>();
}