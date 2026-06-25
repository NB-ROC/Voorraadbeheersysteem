using Backend.Entities;
using Backend.Entities.Relations;
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
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .OrderBy(user => user.Number)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<User?> Get(int id)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    // Replaces all roles for a user atomically.
    public async Task<bool> SetRoles(int userId, IEnumerable<RoleType> roleIds)
    {
        // Remove existing roles for this user
        List<UserRole> existing = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync();

        _context.UserRoles.RemoveRange(existing);

        // Add the new set
        foreach (RoleType roleId in roleIds)
            _context.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = roleId
            });

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

    public async Task<(int id, string email, string name)?> LenderScan(byte[] cardId)
    {
        User? user = await _context.Users
            .Include(u => u.UserRoles)
            .Where(u => u.UserRoles.Any(ur =>
                    ur.Role.Name == "Lender" ||
                    ur.Role.Name == "Manager" ||
                    ur.Role.Name == "Admin"
                )
            )
            .Where(u => u.CardId == cardId)
            .FirstOrDefaultAsync();

        if (user == null) return null;

        return (user.Id, user.Email, user.FirstName + " " + user.LastName);
    }

    public async Task<List<User>> LenderPage(int page, int pageSize)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(u => u.UserRoles.Any(ur =>
                ur.Role.Name == "Student" || ur.Role.Name == "Personnel" || ur.Role.Name == "Guest"))
            .OrderBy(user => user.Number)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}