using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Backend.Database;

public class AppDbContext : DbContext
{
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
        return Environment.GetEnvironmentVariable(key) ?? throw new NullReferenceException("Environment value not found.");
    }

    private static uint GetEnvIntUnsigned(string key)
    {
        string parsable = GetEnv(key);
        return uint.Parse(parsable);
    }
}