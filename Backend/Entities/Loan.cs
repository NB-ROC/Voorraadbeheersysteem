using Backend.Entities.Relations;

namespace Backend.Entities;

public class Loan
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int LenderId { get; set; }
    public User Lender { get; set; } = null!;

    public ICollection<LoanProduct> Products = null!;
    
    public DateTime LoanedAt { get; set; } = DateTime.UtcNow;
    public DateTime DueAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
}