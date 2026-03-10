using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<LoanProduct> LoanProducts => Set<LoanProduct>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        
        string server  = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
        string port    = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
        string database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "storage";
        string username = Environment.GetEnvironmentVariable("DB_USER") ?? "user";
        string password = Environment.GetEnvironmentVariable("DB_PASS") ?? "pass";

        string connectionString = $"server={server};port={port};database={database};user={username};password={password}";
        
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoanProduct>()
            .HasKey(lp => new { lp.ProductId, lp.LoanId });
    }
}