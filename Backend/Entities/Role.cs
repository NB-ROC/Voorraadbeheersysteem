using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class Role
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = null!;
}