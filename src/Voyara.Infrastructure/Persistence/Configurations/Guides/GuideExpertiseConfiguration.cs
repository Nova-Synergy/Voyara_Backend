using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Guides;

namespace Voyara.Infrastructure.Persistence.Configurations.Guides
{
    /// <summary>
    /// Represents the configuration for the GuideExpertise entity type.
    /// </summary>
    public sealed class GuideExpertiseConfiguration : IEntityTypeConfiguration<GuideExpertise>
    {
        /// <summary>
        /// Configures the GuideExpertise entity type.
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<GuideExpertise> builder)
        {
            // Configures the primary key for the GuideExpertise entity.
            builder.HasKey(x => x.Id);

            // Configures the GuideProfileId property for the GuideExpertise entity.
            builder.Property(x => x.GuideProfileId)
               .IsRequired();

            // Configures the CategoryId property for the GuideExpertise entity.
            builder.Property(x => x.CategoryId)
               .IsRequired();

            // Configures the relationship between the GuideExpertise entity and the Category entity.
            builder.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configures the indexes for the GuideProfileId properties of the GuideExpertise entity.
            builder.HasIndex(x => x.GuideProfileId);

            // Configures the indexes for the CategoryId properties of the GuideExpertise entity.
            builder.HasIndex(x => x.CategoryId);
        }
    }
}
