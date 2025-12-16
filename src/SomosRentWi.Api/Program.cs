using System.Text;
using CloudinaryDotNet;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SomosRentWi.Api.Security;
using SomosRentWi.Application.Auth.Interfaces;
using SomosRentWi.Application.Auth.Services;
using SomosRentWi.Application.Cars.Interfaces;
using SomosRentWi.Application.Cars.Services;
using SomosRentWi.Application.Companies.Interfaces;
using SomosRentWi.Application.Companies.Services;
using SomosRentWi.Application.Security;
using SomosRentWi.Application.Rentals.Interfaces;
using SomosRentWi.Application.Rentals.Services;
using SomosRentWi.Application.Security;
using SomosRentWi.Application.Services;
using SomosRentWi.Domain.IRepositories;
using SomosRentWi.Infrastructure;
using SomosRentWi.Infrastructure.Persistence;
using SomosRentWi.Infrastructure.Repositories;
using SomosRentWi.Infrastructure.Services;

// =============================================================
// LOAD ENVIRONMENT VARIABLES
// =============================================================
// Only load .env in Development (not in Docker/Production)
var envPath = "../../.env";
if (File.Exists(envPath))
{
    Env.Load(envPath);
    Console.WriteLine("✅ Loaded environment variables from .env file");
}

// =============================================================
// APP BUILDER
// =============================================================
var builder = WebApplication.CreateBuilder(args);

// =============================================================
// CONFIG: DATABASE
// =============================================================
var host = Environment.GetEnvironmentVariable("DB_HOST");
var port = Environment.GetEnvironmentVariable("DB_PORT");
var user = Environment.GetEnvironmentVariable("DB_USER");
var pass = Environment.GetEnvironmentVariable("DB_PASSWORD");
var dbname = Environment.GetEnvironmentVariable("DB_NAME");
var ssl = Environment.GetEnvironmentVariable("DB_SSL_MODE");

var connectionString =
    $"server={host};port={port};database={dbname};user={user};password={pass};SslMode={ssl};";

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

// =============================================================
// DEPENDENCY INJECTION: REPOSITORIES
// =============================================================
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
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
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "your-secret-key-here";
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
            )
        };
    });

builder.Services.AddAuthorization();

// =============================================================
// CORS
// =============================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
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
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("DATABASE CONNECTION FAILED");
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("DATABASE ERROR:");
        Console.ResetColor();
        Console.WriteLine(ex.Message);
    }

    Console.ResetColor();
}

// =============================================================
// MIDDLEWARE PIPELINE
// =============================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();