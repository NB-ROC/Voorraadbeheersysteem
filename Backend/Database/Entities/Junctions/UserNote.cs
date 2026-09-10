using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database.Entities.Junctions;

[PrimaryKey(nameof(UserId), nameof(NoteId))]
public class UserNote
{
    public ulong UserId { get; set; }

    public ulong NoteId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties

    [DeleteBehavior(DeleteBehavior.Cascade)] [ForeignKey(nameof(UserId))] public User User { get; set; } = null!;

    [DeleteBehavior(DeleteBehavior.Cascade)] [ForeignKey(nameof(NoteId))] public Note Note { get; set; } = null!;
}