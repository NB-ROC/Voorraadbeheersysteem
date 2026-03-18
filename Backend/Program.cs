using Backend.Database;
using Backend.Database.Managers;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

#region DB
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
string env = builder.Environment.EnvironmentName;

builder.Services.AddGrpc();

if (env == "Test")
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("testing")); // TODO: Make this actually work lmao
else
    builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<UserManager>();
#endregion

#region GRPC
WebApplication app = builder.Build();

app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

app.MapGrpcService<UserService>();

app.Run();
#endregion
