namespace Backend.Entities.Relations;

public class LoanProduct
{
    public int LoanId { get; set; }
    public Loan Loan { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}