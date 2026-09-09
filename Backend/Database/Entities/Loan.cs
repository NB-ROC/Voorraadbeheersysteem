using System.ComponentModel.DataAnnotations.Schema;
using Backend.Database.Entities.Junctions;

namespace Backend.Database.Entities;

public class Loan
{
    public ulong Id { get; set; }

    public ulong LenderId { get; set; }

    public ulong BorrowerId { get; set; }

    public DateOnly DueAt { get; set; }

    public DateTime? ReturnedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties

    [ForeignKey(nameof(LenderId))] public User Lender { get; set; } = null!;

    [ForeignKey(nameof(BorrowerId))] public User Borrower { get; set; } = null!;

    public ICollection<LoanProduct> LoanProducts { get; set; } = [];
}