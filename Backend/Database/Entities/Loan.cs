using Backend.Database.Entities.Junctions;

namespace Backend.Database.Entities;

public class Loan
{
    public ulong Id { get; set; }

    public ulong? LenderId { get; set; }

    public ulong? BorrowerId { get; set; }

    public DateOnly DueAt { get; set; }

    public DateTime? ReturnedAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties

    public User? Lender { get; set; }

    public User? Borrower { get; set; }

    public ICollection<LoanProduct> LoanProducts { get; set; } = [];
}