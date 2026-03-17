namespace Backend.Entities;

public class LoanProduct
{
    public int LoanId { get; set; }

    public int ProductId { get; set; }

    public byte Amount { get; set; }

    public Loan Loan { get; set; } = null!;

    public Product Product { get; set; } = null!;
}