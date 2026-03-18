using Backend.Database.Managers;
using Backend.Services.Validation;
using Grpc.Core;
using Shared;

namespace Backend.Services;

public class UserService : Users.UsersBase
{
    private readonly UserValidator _validator;

    public UserService(UserManager manager)
    {
        _validator = new UserValidator(manager);
    }

    public override async Task<UserGetResponse> Get(UserGetRequest request, ServerCallContext context)
    {
        _validator.ValidateGet(request);
        throw new NotImplementedException();
    }

    public override async Task<UserCreateResponse> Create(UserCreateRequest request, ServerCallContext context)
    {
        _validator.ValidateCreate(request);
        throw new NotImplementedException();
    }

    public override async Task<UserModifyResponse> Modify(UserModifyRequest request, ServerCallContext context)
    {
        await _validator.ValidateModify(request);
        throw new NotImplementedException();
    }

    public override async Task<UserDeleteResponse> Delete(UserDeleteRequest request, ServerCallContext context)
    {
        _validator.ValidateDelete(request);
        throw new NotImplementedException();
    }
}