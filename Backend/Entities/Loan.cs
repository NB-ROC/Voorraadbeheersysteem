namespace Backend.Entities;

public class Loan
{
    public int Id { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime BorrowedAt { get; set; }

    public DateTime DueAt { get; set; }

    public DateTime? ReturnedAt { get; set; }

    public User User { get; set; } = null!;

    public ICollection<LoanProduct> LoanProducts { get; set; } = new List<LoanProduct>();
}