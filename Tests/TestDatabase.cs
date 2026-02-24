using System;
using MySqlConnector;

namespace StorageBorrowManagement.Tests;

public static class TestDatabase
{
    
    private static MySqlConnection _connection;
    
    
    // Make sure to run this before conducting any other DB tests
    public static void TestConnection()
    {
        MySqlConnectionStringBuilder builder = new()
        {
            Server = Environment.GetEnvironmentVariable("DB_IP") ?? throw new NullReferenceException(),
            Port =
                Convert.ToUInt32(Environment.GetEnvironmentVariable("DB_PORT") ?? throw new NullReferenceException()),
            Database = Environment.GetEnvironmentVariable("DB_NAME") ?? throw new NullReferenceException(),
            UserID = Environment.GetEnvironmentVariable("DB_USER") ?? throw new NullReferenceException(),
            Password = Environment.GetEnvironmentVariable("DB_PASS") ?? throw new NullReferenceException()
        };

        MySqlConnection conn = new(builder.ConnectionString);

        conn.Open();
        conn.Close();
        
        _connection = conn;
    }
}