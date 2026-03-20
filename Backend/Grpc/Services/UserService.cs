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

        UserPageResponse response = new();

        response.Users.AddRange(users.Select(user => new MetaUser
        {
            Id = ByteString.CopyFrom(user.Id),
            Email = user.Email,
            Name = user.Name,
            Number = user.Number,
            Staff = user.Staff
        }));

        return response;
    }

    public override async Task<UserGetResponse> Get(UserGetRequest request, ServerCallContext context)
    {
        _validator.ValidateGet(request);
        User? user = await _manager.Get(request.Id.ToByteArray());
        if (user == null) throw new RpcException(new Status(StatusCode.NotFound, "Invalid User"));
        return new UserGetResponse
        {
            User = new MetaUser
            {
                Id = ByteString.CopyFrom(user.Id),
                Email = user.Email,
                Name = user.Name,
                Number = user.Number,
                Staff = user.Staff
            }
        };
    }

    public override async Task<UserCreateResponse> Create(UserCreateRequest request, ServerCallContext context)
    {
        _validator.ValidateCreate(request);
        return new UserCreateResponse
        {
            Success = await _manager.Create(new User
            {
                Id = request.Id.ToByteArray(),
                Email = request.Email,
                Name = request.Name,
                Number = (ushort)request.Number,
                Staff = request.Staff
            })
        };
    }

    public override async Task<UserModifyResponse> Modify(UserModifyRequest request, ServerCallContext context)
    {
        User user = await _validator.ValidateModify(request);

        if (request.HasEmail) user.Email = request.Email;
        if (request.HasName) user.Name = request.Name;
        if (request.HasNumber) user.Number = (ushort)request.Number;
        if (request.HasStaff) user.Staff = request.Staff;

        return new UserModifyResponse
        {
            Success = await _manager.Modify(user)
        };
    }

    public override async Task<UserDeleteResponse> Delete(UserDeleteRequest request, ServerCallContext context)
    {
        _validator.ValidateDelete(request);
        return new UserDeleteResponse
        {
            Success = await _manager.Delete(request.Id.ToByteArray())
        };
    }
}