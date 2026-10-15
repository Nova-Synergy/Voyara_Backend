using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Guides;

namespace Voyara.Infrastructure.Persistence.Configurations.Guides
{
    /// <summary>
    /// Represents the configuration for the GuideLanguage entity type.
    /// </summary>
	public sealed class GuideLanguageConfiguration : IEntityTypeConfiguration<GuideLanguage>
	{
        /// <summary>
        /// Configures the GuideLanguage entity type.
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<GuideLanguage> builder)
        {
            // Configures the primary key for the GuideLanguage entity.
            builder.HasKey(x => x.Id);

            // Configures the GuideProfileId property for the GuideLanguage entity.
            builder.Property(x => x.GuideProfileId)
                .IsRequired();

            // Configures the LanguageCode property for the GuideLanguage entity.
            builder.Property(x => x.LanguageCode)
                .IsRequired()
                .HasMaxLength(10);

            // Configures the Proficiency property for the GuideLanguage entity.
            builder.Property(x => x.Proficiency)
                .IsRequired()
                .HasMaxLength(50);

            // Configures a unique index on the GuideProfileId and LanguageCode properties for the GuideLanguage entity.
            builder.HasIndex(x => new { x.GuideProfileId, x.LanguageCode })
                .IsUnique();
        }
    }
}
