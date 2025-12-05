using SomosRentWi.Infrastructure.Data;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

// =============================================================
// LOAD ENVIRONMENT VARIABLES
// =============================================================
Env.Load("../.env");

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

builder.Services.AddDbContext<SomosRentWiDbContext>(options =>
{
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =============================================================
// BUILD APP
// =============================================================
var app = builder.Build();

// =============================================================
// DB CONNECTIVITY CHECK AT STARTUP
// =============================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SomosRentWiDbContext>();

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
            Console.WriteLine("DATABASE CONNECTION FAILE");
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();