using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Guides;

namespace Voyara.Infrastructure.Persistence.Configurations.Guides
{
    /// <summary>
    /// Represents the configuration for the GuideReview entity type.
    /// the GuideProfile relationship is configured in GuideProfileConfiguration.
    /// the GuideRequest relationship is configured in GuideRequestConfiguration.
    /// </summary>
    public sealed class GuideReviewConfiguration : IEntityTypeConfiguration<GuideReview>
    {
        /// <summary>
        /// Configures the entity type for the GuideReview model using the provided builder.
        /// </summary>
        /// <remarks>Call this method within the OnModelCreating method to apply entity configuration for
        /// GuideReview. This includes setting keys, property constraints, and indexes as required by the
        /// model.</remarks>
        /// <param name="builder">The builder used to configure the GuideReview entity type and its properties.</param>
        public void Configure(EntityTypeBuilder<GuideReview> builder)
        {
            // Configure the primary key for the GuideReview entity.
            builder.HasKey(x => x.Id);

            // Configure the Text property for the GuideReview entity.
            builder.Property(x => x.Text)
                .IsRequired()
                .HasMaxLength(2000);

            // Indexes
            builder.HasIndex(x => x.GuideProfileId);
        }
    }
}