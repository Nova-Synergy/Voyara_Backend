using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Guides;
namespace Voyara.Infrastructure.Persistence.Configurations.Guides
{
    /// <summary>
    /// Represents the configuration for the GuidePricing entity type.
    /// </summary>
    public sealed class GuidePricingConfiguration : IEntityTypeConfiguration<GuidePricing>
    {
        /// <summary>
        /// Configures the GuidePricing entity type.
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<GuidePricing> builder)
        {

            // Configures the primary key for the GuidePricing entity.
            builder.HasKey(x => x.Id);

            // Configures the GuideProfileId property for the GuidePricing entity.
            builder.Property(x => x.GuideProfileId)
               .IsRequired();

            // Configures the Unit property for the GuidePricing entity.
            builder.Property(x => x.Unit)
                .HasConversion<short>()
                .IsRequired();
            
            // Configures the Amount property for the GuidePricing entity.
            builder.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            // Configures the Currency property for the GuidePricing entity.
            builder.Property(x => x.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .IsFixedLength();

            // Configures an index on the GuideProfileId property for the GuidePricing entity.
            builder.HasIndex(x => x.GuideProfileId);
        }
    }
}