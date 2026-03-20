using Backend.Database;
using Backend.Database.Managers;
using Backend.Grpc.Services;
using Microsoft.EntityFrameworkCore;

#region DB

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
string env = builder.Environment.EnvironmentName;

builder.Services.AddGrpc();

// TODO: Make this dynamically use the in-mem db when run locally, and the db in docker
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("testing"));
// builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<UserManager>();

#endregion

#region GRPC

WebApplication app = builder.Build();

app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

app.MapGrpcService<UserService>();

app.Run();

#endregion