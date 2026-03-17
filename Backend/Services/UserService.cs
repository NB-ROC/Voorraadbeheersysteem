using Backend.Database;
using Backend.Entities;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Backend.Services;

public class UserService : Users.UsersBase
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public override async Task<GetResponse> Get(GetRequest request, ServerCallContext context)
    {
        List<User> users = await _context.Users
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        GetResponse response = new();

        response.Users.AddRange(users.Select(user => new MetaUser
        {
            Id = ByteString.CopyFrom(user.Id),
            Name = user.Name,
            Email = user.Email,
            Number = user.Number,
            Staff = user.Staff
        }));

        return response;
    }

    public override async Task<CreateResponse> Create(CreateRequest request, ServerCallContext context)
    {

        try
        {
            var user = new User
            {
                Id = request.Id.ToByteArray(),
                Name = request.Name,
                Email = request.Email,
                Number = (ushort)request.Number,
                Staff = request.Staff
            };
            await _context.Users.AddAsync(
                user
            );
            await _context.SaveChangesAsync();
            return new CreateResponse
            {
                Success = true
            };
        }
        catch (Exception)
        {
            return new CreateResponse
            {
                Success = false
            };
        }
    }
}