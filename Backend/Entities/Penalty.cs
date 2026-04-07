namespace Backend.Entities;

public class Penalty
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int LoanId { get; set; }
    public Loan Loan { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool Active { get; set; }
}