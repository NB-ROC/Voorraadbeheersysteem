using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database.Entities.Junctions;

[PrimaryKey(nameof(LoanId), nameof(ProductId))]
public class LoanProduct
{
    public ulong LoanId { get; set; }

    public ulong ProductId { get; set; }

    [Required] public int Amount { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties

    public Loan Loan { get; set; } = null!;

    public Product Product { get; set; } = null!;
}