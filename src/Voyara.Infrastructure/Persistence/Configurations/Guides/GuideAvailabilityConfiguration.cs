using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Guides;

namespace Voyara.Infrastructure.Persistence.Configurations.Guides
{
    /// <summary>
    /// Represents the configuration for the GuideAvailability entity type.
    /// </summary>
    public sealed class GuideAvailabilityConfiguration : IEntityTypeConfiguration<GuideAvailability>
    {
        /// <summary>
        /// Configures the GuideAvailability entity type.
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<GuideAvailability> builder)
        {
            // Configures the primary key for the GuideAvailability entity.
            builder.HasKey(x => x.Id);

            // Configures the GuideProfileId property for the GuideAvailability entity.
            builder.Property(x => x.GuideProfileId)
              .IsRequired();

            // Configures the Day property for the GuideAvailability entity.
            builder.Property(x => x.Day)
                .HasConversion<short>()
                .IsRequired();

            // Configures the StartTime property for the GuideAvailability entity.
            builder.Property(x => x.StartTime)
                .IsRequired();

            // Configures the EndTime property for the GuideAvailability entity.
            builder.Property(x => x.EndTime)
                .IsRequired();

            // Configures the index for the GuideProfileId and Day properties of the GuideAvailability entity.
            builder.HasIndex(x => new { x.GuideProfileId, x.Day });
        }
    }
}
