using Backend.Database;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddScoped<GreeterService>();

var app = builder.Build();

app.MapGrpcService<GreeterService>();

app.Run();