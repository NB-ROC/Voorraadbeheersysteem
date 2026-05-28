using Backend.Database.Managers;
using Backend.Entities;
using Backend.Grpc.Validation;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Protos.User;

namespace Backend.Grpc.Services;

public class UserService : Users.UsersBase
{
    private readonly UserManager _manager;
    private readonly UserValidator _validator;

    public UserService(UserManager manager)
    {
        _manager = manager;
        _validator = new UserValidator(manager);
    }

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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return new UserCreateResponse
        {
            Success = await _manager.Create(user)
        };
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
    public override async Task<UserModifyResponse> Modify(UserModifyRequest request, ServerCallContext context)
    {
        User user = await _validator.ValidateModify(request);

        if (request.HasCardId) user.CardId = request.CardId.ToByteArray();
        if (request.HasFirstName) user.FirstName = request.FirstName;
        if (request.HasLastName) user.LastName = request.LastName;
        if (request.HasEmail) user.Email = request.Email;

        user.UpdatedAt = DateTime.UtcNow;

        return new UserModifyResponse
        {
            Success = await _manager.Modify(user)
        };
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
    public override async Task<UserDeleteResponse> Delete(UserDeleteRequest request, ServerCallContext context)
    {
        return new UserDeleteResponse
        {
            Success = await _manager.Delete(request.Id)
        };
    }

    [AllowAnonymous]
    public override async Task<UserLenderScanResponse> LenderScan(UserLenderScanRequest request,
        ServerCallContext context)
    {
        (string email, string name)? tuple = await _manager.LenderScan(request.CardId.ToByteArray());

        UserLenderScanResponse response = new();

        if (tuple != null)
        {
            response.Email = tuple.Value.email;
            response.Name = tuple.Value.name;
        }

        return response;
    }

    private static MetaUser MapMeta(User user)
    {
        return new MetaUser
        {
            Id = user.Id,
            CardId = ByteString.CopyFrom(user.CardId),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
    }
}