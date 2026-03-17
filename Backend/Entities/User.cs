using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class User
{
    public byte[] Id { get; set; } = new byte[7];

    [MaxLength(255)] public string Email { get; set; } = null!;

    [MaxLength(32)] public string Name { get; set; } = null!;

    public ushort Number { get; set; }

    public bool Staff { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}