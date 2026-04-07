using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class Role
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = null!;

    public ICollection<User> Users { get; set; } = new List<User>();
}