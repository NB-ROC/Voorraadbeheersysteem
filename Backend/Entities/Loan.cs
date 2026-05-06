namespace Backend.Entities;

public class Loan
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public byte[] LenderId { get; set; } = null!;
    public User Lender { get; set; } = null!;

    public DateTime LoanedAt { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
}