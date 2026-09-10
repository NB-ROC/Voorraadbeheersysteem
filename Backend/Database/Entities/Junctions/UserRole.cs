using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database.Entities.Junctions;

[PrimaryKey(nameof(UserId), nameof(RoleId))]
public class UserRole
{
    public ulong UserId { get; set; }

    public ulong RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties

    [DeleteBehavior(DeleteBehavior.Cascade)] [ForeignKey(nameof(UserId))] public User User { get; set; } = null!;

    [DeleteBehavior(DeleteBehavior.Cascade)] [ForeignKey(nameof(RoleId))] public Role Role { get; set; } = null!;
}