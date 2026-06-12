using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database.Managers;

public class AuditLogManager
{
    private readonly AppDbContext _context;

    public AuditLogManager(AppDbContext context)
    {
        _context = context;
    }

    public async Task Log(int actorId, string action, string entityType,
        string entityId, string description)
    {
        _context.AuditLogs.Add(new AuditLog
        {
            TimeStamp = DateTime.UtcNow,
            ActorId = actorId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Description = description
        });
        await _context.SaveChangesAsync();
    }
    public async Task<List<AuditLog>> Page(int page, int pageSize)
    {
        return await _context.AuditLogs
            .Include(a => a.Actor)
            .OrderByDescending(a => a.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}