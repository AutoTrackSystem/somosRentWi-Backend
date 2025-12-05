using Microsoft.EntityFrameworkCore;
using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Infrastructure.Data;

public class SomosRentWiDbContext : DbContext
{
    public SomosRentWiDbContext(DbContextOptions<SomosRentWiDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Car> Cars { get; set; } = null!;
    public DbSet<Rental> Rentals { get; set; } = null!;
    public DbSet<CompanyWallet> CompanyWallets { get; set; } = null!;
    public DbSet<WalletTransaction> WalletTransactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureClient(modelBuilder);
        ConfigureCompany(modelBuilder);
        ConfigureCar(modelBuilder);
        ConfigureRental(modelBuilder);
        ConfigureCompanyWallet(modelBuilder);
        ConfigureWalletTransaction(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        // Obtiene el constructor de configuración para la entidad User.
        var entity = modelBuilder.Entity<User>();

        // Especifica que esta entidad se mapeará a la tabla llamada "Users" en la DB.
        entity.ToTable("Users");

        // Define la propiedad 'Id' como la Clave Primaria (Primary Key).
        entity.HasKey(u => u.Id);

        // Define un índice en la columna 'Email' y asegura que los valores sean únicos.
        entity.HasIndex(u => u.Email).IsUnique();

        // Configura la propiedad 'Email'.
        entity.Property(u => u.Email)
            .IsRequired() // La columna NO permite valores NULL (NOT NULL).
            .HasMaxLength(150); // Establece la longitud máxima del campo a 150 caracteres.

        // Configura la propiedad 'PasswordHash'.
        entity.Property(u => u.PasswordHash)
            .IsRequired(); // La columna NO permite valores NULL (NOT NULL).

        // Configura la propiedad 'Role'.
        entity.Property(u => u.Role)
            .IsRequired(); // La columna NO permite valores NULL (NOT NULL).
    }

    private static void ConfigureClient(ModelBuilder modelBuilder)
    {
        // Obtiene el constructor de configuración para la entidad Client.
        var entity = modelBuilder.Entity<Client>();

        // Mapea la entidad a la tabla "Clients".
        entity.ToTable("Clients");

        // Define 'Id' como la Clave Primaria.
        entity.HasKey(c => c.Id);

        // Configura la propiedad 'FirstName'.
        entity.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        // Configura la propiedad 'LastName'.
        entity.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        // Configura la propiedad 'DocumentNumber'.
        entity.Property(c => c.DocumentNumber)
            .IsRequired()
            .HasMaxLength(50);

        // Configura propiedades opcionales con longitud máxima.
        entity.Property(c => c.PrimaryPhone)
            .HasMaxLength(30);

        entity.Property(c => c.SecondaryPhone)
            .HasMaxLength(30);

        entity.Property(c => c.Address)
            .HasMaxLength(250);

        // Configura la relación de uno a uno (One-to-One) con la entidad User.
        entity.HasOne(c => c.User) // Un Client tiene un User.
            .WithOne() // El User tiene un Client.
            .HasForeignKey<Client>(c => c.UserId) // Define UserId como la Clave Foránea (FK) en la tabla 'Clients'.
            .OnDelete(DeleteBehavior.Restrict); // Evita que se elimine el User si hay un Client asociado.
    }

    private static void ConfigureCompany(ModelBuilder modelBuilder)
    {
        // Obtiene el constructor de configuración para la entidad Company.
        var entity = modelBuilder.Entity<Company>();

        // Mapea la entidad a la tabla "Companies".
        entity.ToTable("Companies");

        // Define 'Id' como la Clave Primaria.
        entity.HasKey(c => c.Id);

        // Configura las propiedades requeridas 'TradeName' y 'NitNumber'.
        entity.Property(c => c.TradeName)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(c => c.NitNumber)
            .IsRequired()
            .HasMaxLength(30);

        // Configura propiedades opcionales con longitud máxima.
        entity.Property(c => c.ContactEmail)
            .HasMaxLength(150);

        entity.Property(c => c.MobilePhone)
            .HasMaxLength(30);

        entity.Property(c => c.Address)
            .HasMaxLength(250);

        entity.Property(c => c.Website)
            .HasMaxLength(200);

        // Configura la relación de uno a uno (One-to-One) con la entidad User.
        entity.HasOne(c => c.User)
            .WithOne()
            .HasForeignKey<Company>(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureCar(ModelBuilder modelBuilder)
    {
        // Obtiene el constructor de configuración para la entidad Car.
        var entity = modelBuilder.Entity<Car>();

        // Mapea la entidad a la tabla "Cars".
        entity.ToTable("Cars");

        // Define 'Id' como la Clave Primaria.
        entity.HasKey(c => c.Id);

        // Configura las propiedades requeridas.
        entity.Property(c => c.Brand)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(c => c.Model)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(c => c.Year)
            .HasMaxLength(10);

        entity.Property(c => c.Plate)
            .IsRequired()
            .HasMaxLength(10);

        // Configura propiedades opcionales.
        entity.Property(c => c.Color)
            .HasMaxLength(50);

        entity.Property(c => c.MainPhotoUrl)
            .HasMaxLength(500);

        // Configura la relación de uno a muchos (One-to-Many) con Company.
        // Un Car pertenece an una Company.
        entity.HasOne(c => c.Company)
            .WithMany() // La Company tiene muchas Cars, pero no definimos la propiedad de navegación inversa aquí.
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureRental(ModelBuilder modelBuilder)
    {
        // Obtiene el constructor de configuración para la entidad Rental.
        var entity = modelBuilder.Entity<Rental>();

        // Mapea la entidad a la tabla "Rentals".
        entity.ToTable("Rentals");

        // Define 'Id' como la Clave Primaria.
        entity.HasKey(r => r.Id);

        // Configura propiedades de tipo decimal con precisión específica (18 dígitos en total, 2 después del punto).
        entity.Property(r => r.TotalPrice)
            .HasColumnType("decimal(18,2)");

        entity.Property(r => r.DepositAmount)
            .HasColumnType("decimal(18,2)");

        // Configura propiedad opcional.
        entity.Property(r => r.ContractPdfUrl)
            .HasMaxLength(500);

        // Configura las relaciones de uno a muchos (One-to-Many) con otras entidades:

        // 1. Relación con Client (una renta tiene un cliente).
        entity.HasOne(r => r.Client)
            .WithMany()
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // 2. Relación con Company (una renta pertenece a una compañía).
        entity.HasOne(r => r.Company)
            .WithMany()
            .HasForeignKey(r => r.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // 3. Relación con Car (una renta es sobre un coche).
        entity.HasOne(r => r.Car)
            .WithMany()
            .HasForeignKey(r => r.CarId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    
    private static void ConfigureCompanyWallet(ModelBuilder modelBuilder)
    {
        // Obtiene el constructor de configuración para la entidad CompanyWallet.
        var entity = modelBuilder.Entity<CompanyWallet>();

        // Mapea la entidad a la tabla "CompanyWallets".
        entity.ToTable("CompanyWallets");

        // Define 'Id' como la Clave Primaria.
        entity.HasKey(w => w.Id);

        // Configura la propiedad 'Balance' como decimal con precisión específica.
        entity.Property(w => w.Balance)
            .HasColumnType("decimal(18,2)");

        // Configura la relación de uno a uno (One-to-One) con Company.
        // Una CompanyWallet pertenece a una Company.
        entity.HasOne(w => w.Company)
            .WithOne()
            .HasForeignKey<CompanyWallet>(w => w.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureWalletTransaction(ModelBuilder modelBuilder)
    {
        // Obtiene el constructor de configuración para la entidad WalletTransaction.
        var entity = modelBuilder.Entity<WalletTransaction>();

        // Mapea la entidad a la tabla "WalletTransactions".
        entity.ToTable("WalletTransactions");

        // Define 'Id' como la Clave Primaria.
        entity.HasKey(t => t.Id);

        // Configura la propiedad 'Amount' como decimal con precisión específica.
        entity.Property(t => t.Amount)
            .HasColumnType("decimal(18,2)");

        // Configura la propiedad 'Description' con longitud máxima.
        entity.Property(t => t.Description)
            .HasMaxLength(250);

        // Configura las relaciones de uno a muchos (One-to-Many):

        // 1. Relación con CompanyWallet (una transacción pertenece a una billetera).
        entity.HasOne(t => t.CompanyWallet)
            .WithMany()
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Restrict);

        // 2. Relación con Company (la transacción está asociada a una compañía).
        entity.HasOne(t => t.Company)
            .WithMany()
            .HasForeignKey(t => t.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}