using Backend.DTOs;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database.Managers;

public class AuthManager
{
    private readonly AppDbContext _context;

    public AuthManager(AppDbContext context)
    {
        _context = context;
    }

    // Zoek een user op via CardId en geef hun rollen terug.
    public async Task<ScanLoginResponse?> FindByCardIdWithRoles(byte[] cardId)
    {
        User? user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.CardId == cardId);

        if (user == null) return null;

        List<string> roles = user.UserRoles
            .Select(ur => ur.Role.Name)
            .ToList();

        return new ScanLoginResponse
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Roles = roles,
            IsLender = user.UserRoles.Any(ur => ur.RoleId == RoleType.Lender)
        };
    }
}