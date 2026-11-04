using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Localization;

namespace Voyara.Infrastructure.Persistence.Configurations.Localization
{

    /// <summary>
    /// Represents the configuration for the Language entity type.
    /// The relationship with DestinationTranslation is configured in DestinationTranslationConfiguration.
    /// </summary>
    public sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        /// <summary>
        /// Configures the Language entity type.
        /// </summary>
        /// <param name="builder">The builder used to configure the Language entity type.</param>
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            // Configures the Language entity (the code is the natural key, for example en, ar or pt-br).
            builder.HasKey(x => x.Code);

            // Configures the Code property.
            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(10);

            // Configures the Name property.
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Configures the IsRtl property.
            builder.Property(x => x.IsRtl)
                .IsRequired();
        }
    }
}
