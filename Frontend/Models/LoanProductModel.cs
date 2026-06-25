namespace Frontend.Models;

public class LoanProductModel
{
    public int LoanId { get; set; }
    public LoanModel LoanModel { get; set; } = null!;

    public int ProductId { get; set; }
    public ProductModel Product { get; set; } = null!;

    public int Amount { get; set; }
    public bool Returned { get; set; }
}