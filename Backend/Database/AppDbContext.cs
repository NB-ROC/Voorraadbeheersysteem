using Backend.Database.Entities;
using Backend.Database.Entities.Junctions;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Backend.Database;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<Loan> Loans { get; set; }
    
    public DbSet<LoanProduct> LoanProducts { get; set; }
    public DbSet<ProductNote> ProductNotes { get; set; }
    public DbSet<ProductRole> ProductRoles { get; set; }
    public DbSet<UserNote> UserNotes { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        #region Junction Composite Keys

        builder.Entity<LoanProduct>()
            .HasKey(x => new { x.LoanId, x.ProductId });

        builder.Entity<ProductNote>()
            .HasKey(x => new { x.ProductId, x.NoteId });
        
        builder.Entity<ProductRole>()
            .HasKey(x => new { x.ProductId, x.RoleId });
        
        builder.Entity<UserNote>()
            .HasKey(x => new { x.UserId, x.NoteId });
        
        builder.Entity<UserRole>()
            .HasKey(x => new { x.UserId, x.RoleId });

        #endregion

        #region User Reference Discrepancies

        builder.Entity<Log>()
            .HasOne(x => x.Invoker)
            .WithMany(x => x.InvokedLogs)
            .HasForeignKey(y => y.InvokerId);
        
        builder.Entity<Log>()
            .HasOne(x => x.Target)
            .WithMany(x => x.TargetLogs)
            .HasForeignKey(y => y.TargetId);
        
        builder.Entity<Loan>()
            .HasOne(x => x.Lender)
            .WithMany(x => x.LentLoans)
            .HasForeignKey(y => y.LenderId);
        
        builder.Entity<Loan>()
            .HasOne(x => x.Borrower)
            .WithMany(x => x.BorrowedLoans)
            .HasForeignKey(y => y.BorrowerId);

        #endregion
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        MySqlConnectionStringBuilder builder = new()
        {
            Server = GetEnv("DB_SERVER"),
            Port = GetEnvIntUnsigned("DB_PORT"),
            Database = GetEnv("DB_NAME"),
            UserID = GetEnv("DB_USERNAME"),
            Password = GetEnv("DB_PASSWORD")
        };

        options.UseMySql(builder.ConnectionString, ServerVersion.AutoDetect(builder.ConnectionString));
    }

    private static string GetEnv(string key)
    {
        return Environment.GetEnvironmentVariable(key) ??
               throw new NullReferenceException("Environment value not found.");
    }

    private static uint GetEnvIntUnsigned(string key)
    {
        string parsable = GetEnv(key);
        return uint.Parse(parsable);
    }
}