using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Infrastructure.Persistence.Configurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Company>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.UserId).IsUnique();

        builder.Property(x => x.TradeName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NitNumber).IsRequired().HasMaxLength(50);

        builder.HasIndex(x => x.NitNumber).IsUnique();

        builder.Property(x => x.ContactEmail).HasMaxLength(150);
        builder.Property(x => x.LandlineNumber).HasMaxLength(30);
        builder.Property(x => x.MobilePhone).HasMaxLength(30);
        builder.Property(x => x.Address).HasMaxLength(250);
        builder.Property(x => x.Website).HasMaxLength(200);

        builder.Property(x => x.CompanyPlan).IsRequired();
        builder.Property(x => x.SubscriptionStatus).IsRequired();

        builder.Property(x => x.StartSubscriptionDate);
        builder.Property(x => x.EndSubscriptionDate);

        builder.HasOne(x => x.Wallet)
            .WithOne(w => w.Company)
            .HasForeignKey<CompanyWallet>(w => w.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Cars)
            .WithOne(c => c.Company)
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}