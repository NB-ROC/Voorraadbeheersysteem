using Backend.Database;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var env = builder.Environment.EnvironmentName;

builder.Services.AddGrpc();

if (env == "Test")
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("testing")); // TODO: Make this actually work lmao
}
else
{
    builder.Services.AddDbContext<AppDbContext>();
}

builder.Services.AddDbContext<AppDbContext>();

WebApplication app = builder.Build();

app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

app.MapGrpcService<GreeterService>();
app.MapGrpcService<UserService>();

app.Run();