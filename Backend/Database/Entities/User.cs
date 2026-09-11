using System.ComponentModel.DataAnnotations;
using Backend.Database.Entities.Junctions;

namespace Backend.Database.Entities;

public class User
{
    public ulong Id { get; set; }

    [MinLength(7)] [MaxLength(7)] public byte[]? CardBytes { get; set; }

    [MinLength(1)] [MaxLength(32)] public string Name { get; set; } = string.Empty;

    [MinLength(1)] [MaxLength(254)] public string Email { get; set; } = string.Empty;

    [MinLength(1)] [MaxLength(255)] public string? PasswordHash { get; set; }

    [Required] public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties

    public ICollection<Log> InvokedLogs { get; set; } = [];
    public ICollection<Log> TargetLogs { get; set; } = [];
    public ICollection<Note> Notes { get; set; } = [];
    public ICollection<Loan> LentLoans { get; set; } = [];
    public ICollection<Loan> BorrowedLoans { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<UserNote> UserNotes { get; set; } = [];
}