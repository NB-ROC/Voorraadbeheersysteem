using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database.Managers;

public class UserManager
{
    private readonly AppDbContext _context;

    public UserManager(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> Page(int page, int pageSize)
    {
        return await _context.Users
            .OrderBy(user => user.Number)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<User?> Get(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<bool> Create(User user)
    {
        _context.Users.Add(user);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> Modify(User user)
    {
        _context.Users.Update(user);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            User? user = await _context.Users.FindAsync(id);
            if (user == null) return true;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return false;
        }

        return true;
    }

    public async Task<User?> FindByEmail(string email)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<(string email, string name)?> LenderScan(byte[] cardId)
    {
        User[] users = (await _context.Users
            .Include(u => u.UserRoles)
            .Where(u => u.UserRoles.Any(ur =>
                    ur.Role.Name == "Lender" ||
                    ur.Role.Name == "Manager" ||
                    ur.Role.Name == "Admin"
                )
            )
            .Where(u => u.CardId == cardId)
            .ToArrayAsync());
        
        if (users.Length == 0) return null;
        
        return (users[0].Email, users[0].FirstName + " " + users[0].LastName);
    }
}