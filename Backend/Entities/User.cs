using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class User
{
    public const int IdLength = 7;
    public const int StaffNumberLength = 6;
    public const int StudentNumberLength = 7;
    public const int EmailLength = 255;
    public const int NameLength = 32;


    public byte[] Id { get; set; } = new byte[IdLength];

    [MaxLength(EmailLength)] public string Email { get; set; } = null!;
    [MaxLength(NameLength)] public string FirstName { get; set; } = null!;
    [MaxLength(NameLength)] public string LastName { get; set; } = null!;
    
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public bool IsBlocked { get; set; }
    
    public DateTime CreatedAt { get; set; } 
    public DateTime UpdatedAt { get; set; }

    public int Number { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}