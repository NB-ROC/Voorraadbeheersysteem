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
            Email = request.Email,
            Name = request.Name,
            Number = request.Number,
            Staff = request.Staff
        };

        bool success = await _manager.Create(user);

        return new UserCreateResponse
        {
            Success = success
        };
    }

    public override async Task<UserModifyResponse> Modify(UserModifyRequest request, ServerCallContext context)
    {
        User user = await _validator.ValidateModify(request);

        if (request.HasEmail)
            user.Email = request.Email;

        if (request.HasName)
            user.Name = request.Name;

        if (request.HasNumber)
            user.Number = (ushort)request.Number;

        if (request.HasStaff)
            user.Staff = request.Staff;

        bool success = await _manager.Modify(user);

        return new UserModifyResponse
        {
            Success = success
        };
    }

    public override async Task<UserDeleteResponse> Delete(UserDeleteRequest request, ServerCallContext context)
    {
        bool success = await _manager.Delete(request.Id.ToByteArray());

        return new UserDeleteResponse
        {
            Success = success
        };
    }

    private static MetaUser MapMeta(User user)
    {
        return new MetaUser
        {
            Id = ByteString.CopyFrom(user.Id),
            Email = user.Email,
            Name = user.Name,
            Number = user.Number,
            Staff = user.Staff
        };
    }
}