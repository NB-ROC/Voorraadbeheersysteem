namespace Backend.Entities;

public class User
{
    public byte[] Id { get; set; } = new byte[7];

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public ushort Number { get; set; }

    public bool Staff { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}