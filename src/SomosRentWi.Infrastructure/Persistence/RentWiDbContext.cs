using Microsoft.EntityFrameworkCore;
using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Infrastructure.Persistence;

public class RentWiDbContext : DbContext
{
    public RentWiDbContext(DbContextOptions<RentWiDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Rental> Rentals => Set<Rental>();
    public DbSet<CompanyWallet> CompanyWallets => Set<CompanyWallet>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();

    public DbSet<AdminWallet> AdminWallets => Set<AdminWallet>();
    public DbSet<AdminWalletTransaction> AdminWalletTransactions => Set<AdminWalletTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentWiDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}