using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Infrastructure.Persistence.Configurations;

public sealed class AdminWalletTransactionConfiguration : IEntityTypeConfiguration<AdminWalletTransaction>
{
    public void Configure(EntityTypeBuilder<AdminWalletTransaction> builder)
    {
        builder.ToTable("AdminWalletTransactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.TransactionType)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(300);

        builder.HasIndex(x => x.AdminWalletId);
    }
}