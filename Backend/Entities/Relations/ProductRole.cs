namespace Backend.Entities.Relations;

public class ProductRole
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public RoleType RoleId { get; set; }
    public Role Role { get; set; } = null!;
}