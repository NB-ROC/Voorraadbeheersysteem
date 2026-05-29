using Backend.Database;
using Backend.Entities;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Protos.Notification;

namespace Backend.Grpc.Services;

public class NotificationService : Notifications.NotificationsBase
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public override async Task<NotificationPageResponse> Page(
        NotificationPageRequest request,
        ServerCallContext context)
    {
        List<Notification> notifications = await _context.Notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        NotificationPageResponse response = new();

        response.Notifications.AddRange(notifications.Select(n =>
            new MetaNotification
            {
                Title = n.Title,
                Description = n.Description
            }));

        return response;
    }

    public override async Task<NotificationCreateResponse> Create(
        NotificationCreateRequest request,
        ServerCallContext context)
    {
        Notification notification = new()
        {
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();

        return new NotificationCreateResponse
        {
            Success = true
        };
    }
}