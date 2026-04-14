using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<LoanProduct> LoanProducts => Set<LoanProduct>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<DefectReport> DefectReport => Set<DefectReport>();
    public DbSet<Penalty> Penalty => Set<Penalty>();
    public DbSet<ProductHistory> ProductHistory => Set<ProductHistory>();
    public DbSet<Role> Role => Set<Role>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        string server = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
        string port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
        string database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "storage";
        string username = Environment.GetEnvironmentVariable("DB_USER") ?? "user";
        string password = Environment.GetEnvironmentVariable("DB_PASS") ?? "pass";

        string connectionString =
            $"server={server};port={port};database={database};user={username};password={password}";

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<LoanProduct>()
        .HasKey(lp => new { lp.ProductId, lp.LoanId });

    modelBuilder.Entity<User>()
        .HasOne(u => u.Role)
        .WithMany()
        .HasForeignKey(u => u.RoleId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Product>()
        .HasOne(p => p.RestrictedRole)
        .WithMany()
        .HasForeignKey(p => p.RestrictedRoleId)
        .OnDelete(DeleteBehavior.SetNull);
    
    modelBuilder.Entity<Loan>()
        .HasOne(l => l.User)
        .WithMany(u => u.Loans)
        .HasForeignKey(l => l.UserId)
        .OnDelete(DeleteBehavior.Restrict);
    
    modelBuilder.Entity<Loan>()
        .HasOne(l => l.Issuer)
        .WithMany()
        .HasForeignKey(l => l.IssuedBy)
        .OnDelete(DeleteBehavior.Restrict);
    
    modelBuilder.Entity<DefectReport>()
        .HasOne(d => d.Reporter)
        .WithMany()
        .HasForeignKey(d => d.ReportedBy)
        .OnDelete(DeleteBehavior.Restrict);
    
    modelBuilder.Entity<Penalty>()
        .HasOne(p => p.User)
        .WithMany()
        .HasForeignKey(p => p.UserId)
        .OnDelete(DeleteBehavior.Restrict);
    
    modelBuilder.Entity<Penalty>()
        .HasOne(p => p.Loan)
        .WithMany()
        .HasForeignKey(p => p.LoanId)
        .OnDelete(DeleteBehavior.Restrict);
    
    modelBuilder.Entity<ProductHistory>()
        .HasOne(ph => ph.Product)
        .WithMany()
        .HasForeignKey(ph => ph.ProductId)
        .OnDelete(DeleteBehavior.Restrict);
    
    modelBuilder.Entity<ProductHistory>()
        .HasOne(ph => ph.Performer)
        .WithMany()
        .HasForeignKey(ph => ph.PerformedBy)
        .OnDelete(DeleteBehavior.Restrict);
}
}