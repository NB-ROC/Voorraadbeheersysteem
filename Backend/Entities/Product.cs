using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class Product
{
    public int Id { get; set; }

    [MaxLength(64)] public string Name { get; set; } = null!;

    [MaxLength(45)] public string Category { get; set; } = null!;

    [MaxLength(128)] public string Description { get; set; } = null!;

    public byte Amount { get; set; }

    public ICollection<LoanProduct> LoanProducts { get; set; } = new List<LoanProduct>();
}