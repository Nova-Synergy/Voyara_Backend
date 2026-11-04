using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Localization;

namespace Voyara.Infrastructure.Persistence.Configurations.Localization
{

    /// <summary>
    /// Represents the configuration for the DestinationTranslation entity type.
    /// The relationship with Destination is configured in DestinationConfiguration.
    /// </summary>
    public sealed class DestinationTranslationConfiguration : IEntityTypeConfiguration<DestinationTranslation>
    {
        /// <summary>
        /// Configures the DestinationTranslation entity type.
        /// </summary>
        /// <param name="builder">The builder used to configure the DestinationTranslation entity type.</param>
        public void Configure(EntityTypeBuilder<DestinationTranslation> builder)
        {
            // Configures the DestinationTranslation entity (one translation per destination and language).
            builder.HasKey(x => new
            {
                x.DestinationId,
                x.LanguageCode
            });

            // Configures the DestinationId property.
            builder.Property(x => x.DestinationId)
                .IsRequired();

            // Configures the LanguageCode property.
            builder.Property(x => x.LanguageCode)
                .IsRequired()
                .HasMaxLength(10);

            // Configures the Name property.
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Configures the Description property.
            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasMaxLength(4000);

            // Configures the CultureNotes property.
            builder.Property(x => x.CultureNotes)
                .IsRequired(false)
                .HasMaxLength(4000);

            // Configures the Language relationship (a language that has translations cannot be hard-deleted).
            builder.HasOne(x => x.Language)
                .WithMany()
                .HasForeignKey(x => x.LanguageCode)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Supports finding all the translations in a language.
            builder.HasIndex(x => x.LanguageCode);
        }
    }
}
