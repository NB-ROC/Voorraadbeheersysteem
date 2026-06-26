using Backend.Entities;
using Backend.Entities.Relations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Role> Role => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<LoanProduct> LoanProducts => Set<LoanProduct>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<ProductRole> ProductRoles => Set<ProductRole>();
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

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

        optionsBuilder
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relation Composite Keys
        modelBuilder.Entity<LoanProduct>()
            .HasKey(lp => new { lp.LoanId, lp.ProductId });
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });
        modelBuilder.Entity<ProductRole>()
            .HasKey(pr => new { pr.ProductId, pr.RoleId });

        // Loan → User (borrower) and User (lender)
        // Two FKs to the same table require explicit naming to avoid ambiguity
        modelBuilder.Entity<Loan>()
            .HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Lender)
            .WithMany()
            .HasForeignKey(l => l.LenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasMany(l => l.Products)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        // Product → Category
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // LoanProduct → Loan
        modelBuilder.Entity<LoanProduct>()
            .HasOne(lp => lp.Loan)
            .WithMany()
            .HasForeignKey(lp => lp.LoanId)
            .OnDelete(DeleteBehavior.Cascade);

        // LoanProduct → Product
        modelBuilder.Entity<LoanProduct>()
            .HasOne(lp => lp.Product)
            .WithMany()
            .HasForeignKey(lp => lp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserRole → User
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserRole → Role
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProductRole → Product
        modelBuilder.Entity<ProductRole>()
            .HasOne(pr => pr.Product)
            .WithMany()
            .HasForeignKey(pr => pr.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<ProductRole>()
            .HasOne(pr => pr.Role)
            .WithMany()
            .HasForeignKey(pr => pr.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Default roles
        modelBuilder.Entity<Role>()
            .HasData(
                new Role(RoleType.Admin),
                new Role(RoleType.Manager),
                new Role(RoleType.Lender),
                new Role(RoleType.Student),
                new Role(RoleType.Personnel),
                new Role(RoleType.Guest)
            );
        modelBuilder.Entity<AuditLog>()
            .HasOne(a => a.Actor)
            .WithMany()
            .HasForeignKey(a => a.ActorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Default category
        modelBuilder.Entity<Category>()
            .HasData(
                new Category
                {
                    Id = 1,
                    Name = "Test"
                }
            );
    }

    public static async Task SeedAsync(IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        byte[] userId = [4, 108, 200, 178, 200, 21, 144];

        if (!context.Users.Any())
        {
            PasswordHasher<User> hasher = new();

            User user = new()
            {
                UserId = userId,
                Number = 123456,
                Email = "testmail@roc-nijmegen.nl",
                PasswordHash = hasher.HashPassword(null!, "Placeholder1"),
                FirstName = "Admin",
                LastName = "Istrator",
                CreatedAt = DateTime.Now
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        if (!context.UserRoles.Any())
        {
            context.UserRoles.Add(new UserRole
            {
                RoleId = RoleType.Admin,
                UserId = 1
            });
            await context.SaveChangesAsync();
        }
    }
}