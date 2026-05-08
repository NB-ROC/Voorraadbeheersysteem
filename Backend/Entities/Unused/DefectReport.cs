namespace Backend.Entities.Unused;

public class DefectReport
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public byte[] ReportedBy { get; set; } = null!;
    public User Reporter { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime ReportedAt { get; set; }

    public bool Resolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
}