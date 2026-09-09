using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database.Entities.Junctions;

[PrimaryKey(nameof(ProductId), nameof(RoleId))]
public class ProductRole
{
    public ulong ProductId { get; set; }

    public ulong RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties

    [ForeignKey(nameof(ProductId))] public Product Product { get; set; } = null!;

    [ForeignKey(nameof(RoleId))] public Role Role { get; set; } = null!;
}