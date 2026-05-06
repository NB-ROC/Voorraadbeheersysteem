using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class Role
{
    public const int NameLength = 32;

    public int Id { get; set; }

    [MaxLength(NameLength)] public string Name { get; set; } = null!;
}