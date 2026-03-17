namespace Backend.Entities;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string Description { get; set; } = null!;

    public byte Amount { get; set; }

    public ICollection<LoanProduct> LoanProducts { get; set; } = new List<LoanProduct>();
}