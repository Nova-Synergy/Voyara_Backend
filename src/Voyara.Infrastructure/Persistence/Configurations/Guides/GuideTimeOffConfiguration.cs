using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Guides;
namespace Voyara.Infrastructure.Persistence.Configurations.Guides
{
    /// <summary>
    /// Represents the configuration for the GuideTimeOff entity type.
    /// </summary>
    public sealed class GuideTimeOffConfiguration : IEntityTypeConfiguration<GuideTimeOff>
    {
        /// <summary>
        /// Configures the GuideTimeOff entity type.
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<GuideTimeOff> builder)
        {
            
            // Configures the primary key for the GuideTimeOff entity.
            builder.HasKey(x => x.Id);

            // Configures the GuideProfileId property for the GuideTimeOff entity.
            builder.Property(x => x.GuideProfileId)
               .IsRequired();

            // Configures the Date property for the GuideTimeOff entity.
            builder.Property(x => x.Date)
                .IsRequired();

            // Configures the Reason property for the GuideTimeOff entity.
            builder.Property(x => x.Reason)
                .HasMaxLength(300);

            // Configures a unique index on the GuideProfileId and Date properties for the GuideTimeOff entity.
            builder.HasIndex(x => new { x.GuideProfileId, x.Date })
                .IsUnique();
        }
    }
}