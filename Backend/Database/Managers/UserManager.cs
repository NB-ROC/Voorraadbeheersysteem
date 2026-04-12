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

    public async Task<User?> Get(byte[] id)
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

    public async Task<bool> Delete(byte[] id)
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
}