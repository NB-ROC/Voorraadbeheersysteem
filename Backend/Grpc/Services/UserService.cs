using System.Security.Claims;
using Backend.Database;
using Backend.Database.Managers;
using Backend.Entities;
using Backend.Grpc.Validation;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Protos.User;
using Role = Protos.Product.Role;

namespace Backend.Grpc.Services;

public class UserService : Users.UsersBase
{
    private readonly UserManager _manager;
    private readonly UserValidator _validator;
    private readonly AppDbContext _context;
    private readonly AuditLogManager _auditLogManager;

    public UserService(UserManager manager, AppDbContext context, AuditLogManager auditLogManager)
    {
        _manager = manager;
        _validator = new UserValidator(manager);
        _context = context;
        _auditLogManager = auditLogManager;
    }

    private static int GetActorId(ServerCallContext context) =>
        int.Parse(context.GetHttpContext().User
            .FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
    public override async Task<UserPageResponse> Page(UserPageRequest request, ServerCallContext context)
    {
        _validator.ValidatePage(request);

        List<User> users = await _manager.Page(request.Page, request.PageSize);

        return new UserPageResponse
        {
            Users = { users.Select(MapMeta) }
        };
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
    public override async Task<UserGetResponse> Get(UserGetRequest request, ServerCallContext context)
    {
        User? user = await _manager.Get(request.Id);

        if (user == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Invalid user"));

        return new UserGetResponse
        {
            User = MapMeta(user)
        };
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
    public override async Task<UserCreateResponse> Create(UserCreateRequest request, ServerCallContext context)
    {
        _validator.ValidateCreate(request);

        User user = new()
        {
            CardId = request.CardId.ToByteArray(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Number = request.Number,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        bool created = await _manager.Create(user);
        if (!created)
            return new UserCreateResponse { Success = false };
        
        Notification notification = new()
        {
            Title = "Nieuwe registratie",
            Description = $"{user.FirstName} {user.LastName} heeft zich geregistreerd.",
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();
        
        if (request.RoleIds.Count > 0)
        {
            IEnumerable<RoleType> roles = request.RoleIds.Select(id => (RoleType)id);
            await _manager.SetRoles(user.Id, roles);
        }

        await _auditLogManager.Log(
            GetActorId(context),
            "CREATE",
            "User",
            user.Id.ToString(),
            $"Gebruiker '{user.FirstName} {user.LastName}' aangemaakt"
        );

        return new UserCreateResponse { Success = true };
    }

    public override async Task<UserModifyResponse> Modify(UserModifyRequest request, ServerCallContext context)
    {
        User user = await _validator.ValidateModify(request);

        if (request.HasCardId) user.CardId = request.CardId.ToByteArray();
        if (request.HasFirstName) user.FirstName = request.FirstName;
        if (request.HasLastName) user.LastName = request.LastName;
        if (request.HasEmail) user.Email = request.Email;
        if (request.HasNumber) user.Number = request.Number;

        user.UpdatedAt = DateTime.UtcNow;

        bool modified = await _manager.Modify(user);
        if (!modified)
            return new UserModifyResponse { Success = false };
        
        if (request.RoleIds.Count > 0)
        {
            IEnumerable<RoleType> roles = request.RoleIds.Select(id => (RoleType)id);
            await _manager.SetRoles(user.Id, roles);
        }

        await _auditLogManager.Log(
            GetActorId(context),
            "UPDATE",
            "User",
            user.Id.ToString(),
            $"Gebruiker '{user.FirstName} {user.LastName}' aangepast"
        );

        return new UserModifyResponse { Success = true };
    }

    public static MetaUser MapMeta(User user)
    {
        MetaUser meta = new()
        {
            Id = user.Id,
            CardId = ByteString.CopyFrom(user.CardId),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Number = user.Number
        };

        meta.Roles.AddRange(user.UserRoles.Select(ur => new Role
        {
            Id = (int)ur.Role.Id,
            Name = ur.Role.Name
        }));

        return meta;
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
    public override async Task<UserDeleteResponse> Delete(UserDeleteRequest request, ServerCallContext context)
    {
        bool success = await _manager.Delete(request.Id);

        if (success)
        {
            await _auditLogManager.Log(
                GetActorId(context),
                "DELETE",
                "User",
                request.Id.ToString(),
                $"Gebruiker verwijderd (id {request.Id})"
            );
        }

        return new UserDeleteResponse { Success = success };
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Lender)}")]
    public override async Task<UserLenderPageResponse> LenderPage(UserLenderPageRequest request,
        ServerCallContext context)
    {
        List<User> users = await _manager.LenderPage(request.Page, request.PageSize);

        return new UserLenderPageResponse
        {
            Users = { users.Select(MapMeta) }
        };
        ;
    }

    [AllowAnonymous]
    public override async Task<UserLenderScanResponse> LenderScan(UserLenderScanRequest request,
        ServerCallContext context)
    {
        (int id, string email, string name)? tuple = await _manager.LenderScan(request.CardId.ToByteArray());

        UserLenderScanResponse response = new();

        if (tuple != null)
        {
            response.Id = tuple.Value.id;
            response.Email = tuple.Value.email;
            response.Name = tuple.Value.name;
        }

        return response;
    }
}