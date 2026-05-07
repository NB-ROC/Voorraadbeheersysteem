using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class Role
{
    public const int NameLength = 32;

    public RoleType Id { get; set; }

    [MaxLength(NameLength)] public string Name { get; set; } = null!;

    public Role()
    {
        
    }

    public Role(RoleType role)
    {
        Id = role;
        Name = Enum.GetName(role)!;
    }
}

public enum RoleType
{
    Admin,
    Manager,
    Lender,
    Student,
    Personnel,
    Guest
}