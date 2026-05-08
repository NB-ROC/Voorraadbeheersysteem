namespace Backend.Entities.Unused;

public class ProductHistory
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string ActionType { get; set; } = null!;

    public byte[] PerformedBy { get; set; } = null!;
    public User Performer { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public string? Notes { get; set; }
}