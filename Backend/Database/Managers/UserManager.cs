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
    
    public async Task<User?> GetUser(byte[] id)
    {
        return await _context.Users.FindAsync(id);
    }
}