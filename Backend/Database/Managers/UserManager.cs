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
            .OrderBy(user => user.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<User?> Get(byte[] id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task Create(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task Modify(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
    
    public async Task Delete(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}