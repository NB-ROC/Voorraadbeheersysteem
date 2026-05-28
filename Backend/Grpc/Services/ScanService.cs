using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Database;
using Backend.Entities;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Protos.Scan;

namespace Backend.Grpc.Services;

public class ScanService : Scans.ScansBase
{
    private readonly AppDbContext _context;

    public ScanService(AppDbContext context)
    {
        _context = context;
    }

    public override async Task<TryLoginResponse> TryLogin(
        TryLoginRequest request, ServerCallContext context)
    {
        byte[] cardId = request.CardId.ToByteArray();

        User? user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.CardId == cardId);

        if (user == null)
            return new TryLoginResponse { Result = LoginResult.NotRegistered };

        // Gebruiker is geblokkeerd
        if (user.IsBlocked)
            return new TryLoginResponse { Result = LoginResult.Invalid };

        List<string> roles = user.UserRoles
            .Select(ur => ur.Role.Name)
            .ToList();

        bool isLender = user.UserRoles
            .Any(ur => ur.RoleId == RoleType.Lender);

        string token = GenerateToken(user, roles);

        TryLoginResponse response = new()
        {
            Result = LoginResult.Succes,
            Token = token,
            IsLender = isLender,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        // repeated field vul je zo in met AddRange
        response.Roles.AddRange(roles);

        return response;
    }

    private static string GenerateToken(User user, List<string> roles)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email)
        ];

        // Voeg elke rol toe als aparte claim zodat [Authorize(Roles = "...")] werkt
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        SymmetricSecurityKey key = new(Program.JwtSecret);
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwt = new(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}