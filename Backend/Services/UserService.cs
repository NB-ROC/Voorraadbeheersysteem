using Backend.Database;
using Backend.Database.Managers;
using Backend.Entities;
using Backend.Services.Validation;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
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
        throw new NotImplementedException();
    }

    public override async Task<UserCreateResponse> Create(UserCreateRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task<UserModifyResponse> Modify(UserModifyRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task<UserDeleteResponse> Delete(UserDeleteRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}