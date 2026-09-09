using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database.Entities.Junctions;

[PrimaryKey(nameof(ProductId), nameof(NoteId))]
public class ProductNote
{
    public ulong ProductId { get; set; }

    public ulong NoteId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties

    [ForeignKey(nameof(ProductId))] public Product Product { get; set; } = null!;

    [ForeignKey(nameof(NoteId))] public Note Note { get; set; } = null!;
}