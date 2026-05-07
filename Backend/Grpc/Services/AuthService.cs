using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Database.Managers;
using Backend.Entities;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Protos.Auth;

namespace Backend.Grpc.Services;

public class AuthService : Auth.AuthBase
{
    private UserManager _userManager;
    private PasswordHasher<User> _passwordHasher = new();

    public AuthService(UserManager userManager)
    {
        _userManager = userManager;
    }
    
    [AllowAnonymous]
    public override async Task<AuthLoginResponse> Login(AuthLoginRequest request, ServerCallContext context)
    {
        User? user = await _userManager.FindByEmail(request.Email);

        if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password ) == PasswordVerificationResult.Failed)
        {
            Console.WriteLine("Invalid");
            return new AuthLoginResponse();
        }
        
        return new AuthLoginResponse
        {
            Token = GenerateToken(user, user.UserRoles.Select(userRole => userRole.Role.Name).ToList()),
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