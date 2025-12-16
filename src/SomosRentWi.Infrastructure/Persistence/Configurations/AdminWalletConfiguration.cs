using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Infrastructure.Persistence.Configurations;

public sealed class AdminWalletConfiguration : IEntityTypeConfiguration<AdminWallet>
{
    public void Configure(EntityTypeBuilder<AdminWallet> builder)
    {
        builder.ToTable("AdminWallets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Balance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasMany(x => x.Transactions)
            .WithOne(t => t.AdminWallet)
            .HasForeignKey(t => t.AdminWalletId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed: un solo wallet global
        builder.HasData(new AdminWallet
        {
            Id = 1,
            Balance = 0m
        });
    }
}