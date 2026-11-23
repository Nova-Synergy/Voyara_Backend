using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Geography;

namespace Voyara.Infrastructure.Persistence.Configurations.Geography
{

    /// <summary>
    /// Represents the configuration for the City entity type.
    /// The relationship with Country is configured in CountryConfiguration.
    /// </summary>
    public sealed class CityConfiguration : IEntityTypeConfiguration<City>
    {
        /// <summary>
        /// Configures the City entity type.
        /// </summary>
        /// <param name="builder">The builder used to configure the City entity type.</param>
        public void Configure(EntityTypeBuilder<City> builder)
        {
            // Configures the City entity.
            builder.HasKey(x => x.Id);

            // Configures the Latitude property (degrees, -90 to 90).
            builder.Property(x => x.Latitude)
                .IsRequired();

            // Configures the Longitude property (degrees, -180 to 180).
            builder.Property(x => x.Longitude)
                .IsRequired();

            // Configures the IsActive property.
            builder.Property(x => x.IsActive)
                .IsRequired();

            // Configures the CountryId property.
            builder.Property(x => x.CountryId)
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

            // Hides soft-deleted cities from every query.
            builder.HasQueryFilter(x => !x.IsDeleted);

            // Supports listing the active cities of a country.
            builder.HasIndex(x => new
            {
                x.CountryId,
                x.IsActive
            });
        }
    }
}
