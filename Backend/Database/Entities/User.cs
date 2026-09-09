using System.ComponentModel.DataAnnotations;
using Backend.Database.Entities.Junctions;

namespace Backend.Database.Entities;

public class User
{
    public ulong Id { get; set; }

    [MinLength(7)] [MaxLength(7)] public byte[] CardBytes { get; set; } = new byte[7];

    [MinLength(1)] [MaxLength(32)] public string Name { get; set; } = string.Empty;

    [MinLength(1)] [MaxLength(254)] public string Email { get; set; } = string.Empty;

    [MinLength(1)] [MaxLength(255)] public string? PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties

    public ICollection<Log> Logs { get; set; } = [];
    public ICollection<Note> Notes { get; set; } = [];
    public ICollection<Loan> Loans { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<UserNote> UserNotes { get; set; } = [];
}