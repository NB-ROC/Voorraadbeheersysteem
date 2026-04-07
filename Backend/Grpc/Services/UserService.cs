using Backend.Database.Managers;
using Backend.Entities;
using Backend.Grpc.Validation;
using Google.Protobuf;
using Grpc.Core;
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

    public override async Task<UserPageResponse> Page(UserPageRequest request, ServerCallContext context)
    {
        _validator.ValidatePage(request);

        List<User> users = await _manager.Page(request.Page, request.PageSize);

        return new UserPageResponse
        {
            Users = { users.Select(MapMeta) }
        };
    }

    public override async Task<UserGetResponse> Get(UserGetRequest request, ServerCallContext context)
    {
        User? user = await _manager.Get(request.Id.ToByteArray());

        if (user == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Invalid user"));

        return new UserGetResponse
        {
            User = MapMeta(user)
        };
    }

    public override async Task<UserCreateResponse> Create(UserCreateRequest request, ServerCallContext context)
    {
        _validator.ValidateCreate(request);

        User user = new()
        {
            Id = request.Id.ToByteArray(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            RoleId = request.RoleId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return new UserCreateResponse
        {
            Success = await _manager.Create(user)
        };
    }

    public override async Task<UserModifyResponse> Modify(UserModifyRequest request, ServerCallContext context)
    {
        User user = await _validator.ValidateModify(request);

        if (request.HasFirstName) user.FirstName = request.FirstName;
        if (request.HasLastName) user.LastName = request.LastName;
        if (request.HasEmail) user.Email = request.Email;
        if (request.HasRoleId) user.RoleId = request.RoleId;

        user.UpdatedAt = DateTime.UtcNow;

        return new UserModifyResponse
        {
            Success = await _manager.Modify(user)
        };
    }

    public override async Task<UserDeleteResponse> Delete(UserDeleteRequest request, ServerCallContext context)
    {
        return new UserDeleteResponse
        {
            Success = await _manager.Delete(request.Id.ToByteArray())
        };
    }

    private static MetaUser MapMeta(User user)
    {
        return new MetaUser
        {
            Id = ByteString.CopyFrom(user.Id),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            RoleId = user.RoleId
        };
    }
}