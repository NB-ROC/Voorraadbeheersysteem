using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database.Entities.Junctions;

[PrimaryKey(nameof(LoanId), nameof(ProductId))]
public class LoanProduct
{
    public ulong LoanId { get; set; }

    public ulong ProductId { get; set; }

    [Required] public int Amount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties

    [ForeignKey(nameof(LoanId))] public Loan Loan { get; set; } = null!;

    [ForeignKey(nameof(ProductId))] public Product Product { get; set; } = null!;
}