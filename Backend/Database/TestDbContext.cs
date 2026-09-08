using Microsoft.EntityFrameworkCore;

namespace Backend.Database;

public class TestDbContext : AppDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        const string relativePath = "Database/database.sqlite";

        if (!File.Exists(relativePath))
        {
            using (File.Create(relativePath)) {}
        }
        
        options.UseSqlite("Data Source=" + relativePath);
    }
}