using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Infrastructure.Persistence.Configurations;

public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Client>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.UserId).IsUnique();

        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);

        builder.Property(x => x.DocumentType).IsRequired();
        builder.Property(x => x.DocumentNumber).IsRequired().HasMaxLength(50);

        builder.HasIndex(x => new { x.DocumentType, x.DocumentNumber }).IsUnique();

        builder.Property(x => x.BirthDate).IsRequired();

        builder.Property(x => x.PrimaryPhone).IsRequired().HasMaxLength(30);
        builder.Property(x => x.SecondaryPhone).HasMaxLength(30);
        builder.Property(x => x.Address).HasMaxLength(250);

        builder.Property(x => x.VerificationStatus).IsRequired();
        builder.Property(x => x.HistoryStatus).IsRequired();

        builder.Property(x => x.UrlSelfiePhoto).HasMaxLength(600);
        builder.Property(x => x.UrlDocumentFront).HasMaxLength(600);
        builder.Property(x => x.UrlDocumentBack).HasMaxLength(600);
        builder.Property(x => x.UrlLicenseFront).HasMaxLength(600);
        builder.Property(x => x.UrlLicenseBack).HasMaxLength(600);
    }
}