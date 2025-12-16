using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SomosRentWi.Application.Security;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Role)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        // Seed: Admin user
        var passwordHasher = new PasswordHasher();
        builder.HasData(new User
        {
            Id = 1,
            Email = "admin@somosrentwi.com",
            PasswordHash = passwordHasher.Hash("Admin123!"),
            Role = UserRole.Admin,
            IsActive = true
        });
    }
}