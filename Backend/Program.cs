using Backend.Database;
using Backend.Database.Managers;
using Backend.Grpc.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

#region DB

// TODO: Make it so it dynamically creates these at runtime to avoid errors
Directory.CreateDirectory("Storage");
Directory.CreateDirectory("Storage/Products");

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
string env = builder.Environment.EnvironmentName;

builder.Services.AddGrpc();

// TODO: Make this dynamically use the in-mem db when run locally, and the db in docker
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("testing"));
// builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<ProductManager>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                // TODO: Make this an environment variable
                new SymmetricSecurityKey("super-secret-key-temp"u8.ToArray())
        };
    });

builder.Services.AddAuthorization();

#endregion

#region GRPC

WebApplication app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

app.MapGrpcService<UserService>();
app.MapGrpcService<ProductService>();

app.Run();

#endregion