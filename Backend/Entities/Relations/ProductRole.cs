using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entities.Relations;

public class ProductRole
{
    public int ProductId { get; set; }
    public RoleType RoleId { get; set; }
    
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;

    [ForeignKey(nameof(RoleId))]
    public Role Role { get; set; } = null!;
}