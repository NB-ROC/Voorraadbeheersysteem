using Microsoft.EntityFrameworkCore;

namespace Backend.Database;

public class TestDbContext : AppDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        
    }
}