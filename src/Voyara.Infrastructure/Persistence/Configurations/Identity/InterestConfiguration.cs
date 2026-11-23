using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Identity;

namespace Voyara.Infrastructure.Persistence.Configurations.Identity
{

    /// <summary>
    /// Represents the configuration for the Interest entity type.
    /// The many-to-many relationship with TouristProfile is configured in TouristProfileConfiguration.
    /// </summary>
    public sealed class InterestConfiguration : IEntityTypeConfiguration<Interest>
    {
        /// <summary>
        /// Configures the Interest entity type.
        /// </summary>
        /// <param name="builder">The builder used to configure the Interest entity type.</param>
        public void Configure(EntityTypeBuilder<Interest> builder)
        {
            // Configures the Interest entity.
            builder.HasKey(x => x.Id);

            // Configures the Code property.
            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            // Guarantees that interest codes are unique.
            builder.HasIndex(x => x.Code)
                .IsUnique();
        }
    }
}
