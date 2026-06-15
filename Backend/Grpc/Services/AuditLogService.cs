using System.Security.Claims;
using Backend.Database.Managers;
using Backend.Entities;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Protos.AuditLog;

namespace Backend.Grpc.Services;

[Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)},{nameof(RoleType.Lender)}")]
public class AuditLogService : AuditLogs.AuditLogsBase
{
    private readonly AuditLogManager _manager;
    public AuditLogService(AuditLogManager manager) => _manager = manager;

    public override async Task<AuditLogPageResponse> Page(
        AuditLogPageRequest request, ServerCallContext context)
    {
        var logs = await _manager.Page(request.Page, request.PageSize);
        var response = new AuditLogPageResponse { Total = logs.Count };
        response.Logs.AddRange(logs.Select(l => new MetaAuditLog
        {
            Id = l.Id,
            Timestamp = l.TimeStamp.ToString("o"),
            ActorName = l.Actor.FirstName + " " + l.Actor.LastName,
            Action = l.Action,
            EntityType = l.EntityType,
            EntityId = l.EntityId,
            Description = l.Description
        }));
        return response;
    }
    private int GetActorId(ServerCallContext context) =>
        int.Parse(context.GetHttpContext().User
            .FindFirst(ClaimTypes.NameIdentifier)!.Value);
}