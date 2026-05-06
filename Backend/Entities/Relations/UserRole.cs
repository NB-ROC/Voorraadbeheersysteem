namespace Backend.Entities.Relations;

public class UserRole
{
    public byte[] UserId { get; set; } = null!;
    public User User { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}