using System.Text;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SomosRentWi.Api.Security;
using SomosRentWi.Application.Auth.Interfaces;
using SomosRentWi.Application.Auth.Services;
using SomosRentWi.Application.Wallets.Interfaces;
using SomosRentWi.Application.Wallets.Services;
using SomosRentWi.Application.Cars.Interfaces;
using SomosRentWi.Application.Cars.Services;
using SomosRentWi.Application.Companies.Interfaces;
using SomosRentWi.Application.Companies.Services;
using SomosRentWi.Application.Rentals.Interfaces;
using SomosRentWi.Application.Rentals.Services;
using SomosRentWi.Application.Services;
using SomosRentWi.Application.Security;
using SomosRentWi.Application.Clients.Interfaces;
using SomosRentWi.Application.Clients.Services;
using SomosRentWi.Domain.IRepositories;
using SomosRentWi.Infrastructure;
using SomosRentWi.Infrastructure.Persistence;
using SomosRentWi.Infrastructure.Repositories;
using SomosRentWi.Infrastructure.Services;

// =============================================================
// APP BUILDER
// =============================================================
var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("🚀 Starting SomosRentWi API - Version: Admin Fix (ID 4)");

// Load .env file only in Development (for local development)
if (builder.Environment.IsDevelopment())
{
    var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
    if (File.Exists(envPath))
    {
        Console.WriteLine("✅ Loading .env file for local development");
        foreach (var line in File.ReadAllLines(envPath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
            
            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim().Trim('"');
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}

// =============================================================
// CONFIG: DATABASE
// =============================================================
var host = Environment.GetEnvironmentVariable("DB_HOST");
var port = Environment.GetEnvironmentVariable("DB_PORT");
var user = Environment.GetEnvironmentVariable("DB_USER");
var pass = Environment.GetEnvironmentVariable("DB_PASSWORD");
var dbname = Environment.GetEnvironmentVariable("DB_NAME");
var ssl = Environment.GetEnvironmentVariable("DB_SSL_MODE");

// Debug logging for Railway
Console.WriteLine($"🔍 DB_HOST: {(string.IsNullOrEmpty(host) ? "NOT SET" : host)}");
Console.WriteLine($"🔍 DB_PORT: {(string.IsNullOrEmpty(port) ? "NOT SET" : port)}");
Console.WriteLine($"🔍 DB_NAME: {(string.IsNullOrEmpty(dbname) ? "NOT SET" : dbname)}");
Console.WriteLine($"🔍 DB_USER: {(string.IsNullOrEmpty(user) ? "NOT SET" : "***")}");

var connectionString =
    $"server={host};port={port};database={dbname};user={user};password={pass};SslMode={ssl}";

builder.Services.AddDbContext<RentWiDbContext>(options =>
{
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));
});

// =============================================================
// CONFIG: CLOUDINARY (Optional - only needed for photo uploads)
// =============================================================
var cloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME");
var apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
var apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");

Console.WriteLine($"🔍 CLOUDINARY_CLOUD_NAME: {(string.IsNullOrEmpty(cloudName) ? "NOT SET" : cloudName)}");
Console.WriteLine($"🔍 CLOUDINARY_API_KEY: {(string.IsNullOrEmpty(apiKey) ? "NOT SET" : "***")}");

if (!string.IsNullOrEmpty(cloudName) && !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(apiSecret))
{
    var cloudinaryAccount = new Account(cloudName, apiKey, apiSecret);
    var cloudinary = new Cloudinary(cloudinaryAccount);
    builder.Services.AddSingleton(cloudinary);
    Console.WriteLine("✅ Cloudinary configured successfully");
}
else
{
    // Use a null implementation for development without Cloudinary
    builder.Services.AddSingleton<Cloudinary>(sp => null!);
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("⚠️  WARNING: Cloudinary not configured. Photo upload will not work.");
    Console.WriteLine("   Configure CLOUDINARY_* environment variables in .env file.");
    Console.ResetColor();
}

// =============================================================
// DEPENDENCY INJECTION: SERVICES
// =============================================================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAdminClientService, AdminClientService>();
builder.Services.AddScoped<IWalletQueryService, WalletQueryService>();

// =============================================================
// DEPENDENCY INJECTION: REPOSITORIES
// =============================================================
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAdminWalletRepository, AdminWalletRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// =============================================================
// CONTROLLERS
// =============================================================
builder.Services.AddControllers();

// =============================================================
// SWAGGER/OPENAPI
// =============================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SomosRentWi API",
        Version = "v1",
        Description = "API for car rental management system"
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// =============================================================
// JWT AUTHENTICATION
// =============================================================
// =============================================================
// JWT AUTHENTICATION
// =============================================================
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");

if (string.IsNullOrEmpty(jwtSecret))
{
    if (builder.Environment.IsDevelopment())
    {
        jwtSecret = "development-only-secret-key-must-be-very-long-safe";
        Console.WriteLine("⚠️ Using development JWT secret fallback");
    }
    else
    {
        throw new Exception("CRITICAL: JWT_SECRET environment variable is missing for production deployment.");
    }
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "SomosRentWi",
            ValidAudience = "SomosRentWi",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)
            ),
            RoleClaimType = System.Security.Claims.ClaimTypes.Role // Explicitly map role claim
        };
    });

builder.Services.AddAuthorization();

// =============================================================
// CORS
// =============================================================
var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(',') ?? new[] { "http://localhost:3000", "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", policy => // Renamed for truthfulness
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// =============================================================
// BUILD APP
// =============================================================
var app = builder.Build();

// =============================================================
// DB CONNECTIVITY CHECK AT STARTUP
// =============================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RentWiDbContext>();

    Console.WriteLine("Checking database connection...");

    try
    {
        if (db.Database.CanConnect())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("DATABASE CONNECTED SUCCESSFULLY");
            Console.ResetColor();
            
            // Apply pending migrations automatically
            Console.WriteLine("Applying pending migrations...");
            db.Database.Migrate();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("MIGRATIONS APPLIED SUCCESSFULLY");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("DATABASE CONNECTION FAILED");
            Console.ResetColor();
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("DATABASE ERROR:");
        Console.ResetColor();
        Console.WriteLine(ex.Message);
    }
}

// =============================================================
// MIDDLEWARE PIPELINE
// =============================================================
// Enable Swagger in all environments (including Production for Railway)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SomosRentWi API v1");
    c.RoutePrefix = "swagger"; // Access at /swagger
});

app.UseCors("AllowedOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();