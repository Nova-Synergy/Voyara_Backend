using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Geography;

namespace Voyara.Infrastructure.Persistence.Configurations.Geography
{

    /// <summary>
    /// Represents the configuration for the Country entity type.
    /// </summary>
    public sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        /// <summary>
        /// Configures the Country entity type.
        /// </summary>
        /// <param name="builder">The builder used to configure the Country entity type.</param>
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            // Configures the Country entity.
            builder.HasKey(x => x.Id);

            // Configures the IsoCode property (ISO 3166-1 alpha-2, for example EG).
            builder.Property(x => x.IsoCode)
                .IsRequired()
                .HasMaxLength(2)
                .IsFixedLength();

            // Configures the DefaultCurrency property (ISO 4217, for example EGP).
            builder.Property(x => x.DefaultCurrency)
                .IsRequired()
                .HasMaxLength(3)
                .IsFixedLength();

            // Configures the TimeZone property (IANA identifier, for example Africa/Cairo).
            builder.Property(x => x.TimeZone)
                .IsRequired()
                .HasMaxLength(100);

            // Configures the IsActive property.
            builder.Property(x => x.IsActive)
                .IsRequired();

            // Configures the IsDeleted property.
            builder.Property(x => x.IsDeleted)
                .IsRequired();

            // Configures the CreatedBy property.
            builder.Property(x => x.CreatedBy)
                .IsRequired(false)
                .HasMaxLength(100);

            // Configures the UpdatedBy property.
            builder.Property(x => x.UpdatedBy)
                .IsRequired(false)
                .HasMaxLength(100);

            // Configures the Cities relationship (a country that has cities cannot be hard-deleted).
            builder.HasMany(x => x.Cities)
                .WithOne(x => x.Country)
                .HasForeignKey(x => x.CountryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Hides soft-deleted countries from every query.
            builder.HasQueryFilter(x => !x.IsDeleted);

            // Guarantees that ISO codes are unique. Soft-deleted rows do not block a new entry.
            builder.HasIndex(x => x.IsoCode)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");
        }
    }
}
