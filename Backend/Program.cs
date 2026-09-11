using Backend.Database;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IWebHostEnvironment env = builder.Environment;

if (env.IsDevelopment())
    builder.Services.AddDbContext<AppDbContext, TestDbContext>();
else
    builder.Services.AddDbContext<AppDbContext>();

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        if (dbContext.Database.CanConnect())
        {
            Console.WriteLine("🚀 Database connection verification: SUCCESS!");
            dbContext.Database.EnsureCreated();
        }
        else
        {
            Console.WriteLine("❌ Database connection verification: FAILED (Database might not exist).");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"💥 Database connection verification: CRASHED! Error: {ex.Message}");
    }
}

app.Run();