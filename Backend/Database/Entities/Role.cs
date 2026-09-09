using System.ComponentModel.DataAnnotations;
using Backend.Database.Entities.Junctions;

namespace Backend.Database.Entities;

public enum RoleTree
{
}

public class Role
{
    public ulong Id { get; set; }

    [Required] public RoleTree TreeId { get; set; }

    [MinLength(1)] [MaxLength(16)] public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<ProductRole> ProductRoles { get; set; } = [];
}