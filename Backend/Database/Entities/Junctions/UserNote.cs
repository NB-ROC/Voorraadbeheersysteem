using Microsoft.EntityFrameworkCore;

namespace Backend.Database.Entities.Junctions;

[PrimaryKey(nameof(UserId), nameof(NoteId))]
public class UserNote
{
    public ulong UserId { get; set; }

    public ulong NoteId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties

    public User User { get; set; } = null!;

    public Note Note { get; set; } = null!;
}