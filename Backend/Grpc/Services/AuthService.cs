using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Database.Managers;
using Backend.Entities;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Protos.Auth;
using Protos.User;

namespace Backend.Grpc.Services;

public class AuthService : Auth.AuthBase
{
    private readonly PasswordHasher<User> _passwordHasher = new();
    private readonly UserManager _userManager;

    public AuthService(UserManager userManager)
    {
        _userManager = userManager;
    }

    [AllowAnonymous]
    public override async Task<AuthLoginResponse> Login(AuthLoginRequest request, ServerCallContext context)
    {
        User? user = await _userManager.FindByEmail(request.Email);

        if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password) ==
            PasswordVerificationResult.Failed) return new AuthLoginResponse();

        return new AuthLoginResponse
        {
            Token = GenerateToken(user, user.UserRoles.Select(userRole => userRole.Role.Name).ToList()),
            User = new MetaUser
            {
                Id = user.Id,
                CardId = ByteString.CopyFrom(user.CardId),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            }
        };
    }

    public string GenerateToken(User user, List<string> roles)
    {
        Console.WriteLine(string.Join(',', roles));
        SymmetricSecurityKey key = new(Program.JwtSecret);
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        JwtSecurityToken token = new(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}