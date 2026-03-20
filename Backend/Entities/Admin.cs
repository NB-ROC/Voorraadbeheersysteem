using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class Admin
{
    public int Id { get; set; }

    [MaxLength(32)] public string Username { get; set; } = null!;

    [MaxLength(255)] public string Email { get; set; } = null!;

    [MaxLength(84)] public string PasswordHash { get; set; } = null!;
}