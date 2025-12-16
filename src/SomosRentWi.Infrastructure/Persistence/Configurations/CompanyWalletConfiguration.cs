using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Infrastructure.Persistence.Configurations;

public sealed class CompanyWalletConfiguration : IEntityTypeConfiguration<CompanyWallet>
{
    public void Configure(EntityTypeBuilder<CompanyWallet> builder)
    {
        builder.ToTable("CompanyWallets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Balance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasIndex(x => x.CompanyId).IsUnique();

        builder.HasMany(x => x.Transactions)
            .WithOne(t => t.CompanyWallet)
            .HasForeignKey(t => t.CompanyWalletId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}