using Backend.Database;
using Backend.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddScoped<GreeterService>();

WebApplication app = builder.Build();

app.MapGrpcService<GreeterService>();

app.Run();