namespace Backend.Entities;

public class Loan
{
    public int Id { get; set; }

    public byte[] UserId { get; set; } = null!;
    public User User { get; set; } = null!; 
    
    public int IssuedBy { get; set; } //uitleenmedewerker
    public User Issuer { get; set; } = null!;
    
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    
    public LoanStatus Status { get; set; }
    
    public ICollection<LoanProduct> LoanProducts { get; set; } = new List<LoanProduct>();
}