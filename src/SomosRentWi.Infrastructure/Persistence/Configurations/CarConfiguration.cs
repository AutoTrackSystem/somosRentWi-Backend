using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Infrastructure.Persistence.Configurations;

public sealed class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("Cars");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Brand).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Model).IsRequired().HasMaxLength(100);

        builder.Property(x => x.Year).IsRequired();

        builder.Property(x => x.Plate).IsRequired().HasMaxLength(20);
        builder.HasIndex(x => x.Plate).IsUnique();

        builder.Property(x => x.Color).HasMaxLength(50);

        builder.Property(x => x.CommercialValue)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.PricePerHour)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.SoatExpirationDate).IsRequired();
        builder.Property(x => x.TechExpirationDate).IsRequired();

        builder.Property(x => x.MainPhotoUrl).HasMaxLength(600);

        builder.Property(x => x.Status).IsRequired();

        builder.HasIndex(x => x.CompanyId);
    }
}