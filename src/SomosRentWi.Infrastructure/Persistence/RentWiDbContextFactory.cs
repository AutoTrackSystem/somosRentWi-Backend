using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SomosRentWi.Infrastructure.Persistence;

public class RentWiDbContextFactory 
    : IDesignTimeDbContextFactory<RentWiDbContext>
{
    public RentWiDbContext CreateDbContext(string[] args)
    {
        Env.Load();

        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var port = Environment.GetEnvironmentVariable("DB_PORT");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var pass = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var dbname = Environment.GetEnvironmentVariable("DB_NAME");
        var ssl = Environment.GetEnvironmentVariable("DB_SSL_MODE");

        var connectionString =
            $"server={host};port={port};database={dbname};user={user};password={pass};SslMode={ssl};";

        var options = new DbContextOptionsBuilder<RentWiDbContext>()
            .UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 0))
            )
            .Options;

        return new RentWiDbContext(options);
    }
}