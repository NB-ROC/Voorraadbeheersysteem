using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Database.Entities.Junctions;

namespace Backend.Database.Entities;

public enum RoleTree
{
    Permission,
    Department
}

public class Role
{
    public ulong Id { get; set; }
    
    public ulong? ParentId { get; set; }

    [Required] public RoleTree Tree { get; set; }

    [MinLength(1)] [MaxLength(16)] public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties

    [ForeignKey(nameof(ParentId))]
    public Role? Parent { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<ProductRole> ProductRoles { get; set; } = [];
}