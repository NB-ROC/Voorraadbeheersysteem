using System.ComponentModel.DataAnnotations;
using Backend.Entities.Relations;

namespace Backend.Entities;

public class User
{
    public const int IdLength = 7;
    public const int StaffNumberLength = 6;
    public const int StudentNumberLength = 7;
    public const int EmailLength = 255;
    public const int NameLength = 32;
    private const int PasswordHashLength = 84;

    public int Id { get; set; }
    
    public byte[] CardId { get; set; } = new byte[IdLength];

    public int Number { get; set; }

    public bool IsBlocked { get; set; }

    [MaxLength(EmailLength)] public string Email { get; set; } = null!;

    [MaxLength(NameLength)] public string FirstName { get; set; } = null!;

    [MaxLength(NameLength)] public string LastName { get; set; } = null!;

    [MaxLength(PasswordHashLength)] public string? PasswordHash { get; set; } = null;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}