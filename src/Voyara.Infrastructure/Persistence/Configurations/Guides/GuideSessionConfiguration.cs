using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Guides;

namespace Voyara.Infrastructure.Persistence.Configurations.Guides;

/// <summary>
/// Configures the GuideSession entity and its database mapping.
/// The relationship with GuideRequest is configured in GuideRequestConfiguration.
/// </summary>
public sealed class GuideSessionConfiguration : IEntityTypeConfiguration<GuideSession>
{
    /// <summary>
    /// Configures the primary key and property constraints for GuideSession.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<GuideSession> builder)
    {
        // Configures the primary key for the GuideSession entity.
        builder.HasKey(x => x.Id);

        // Configures the MeetingLocation property for the GuideSession entity.
        builder.Property(x => x.MeetingLocation)
            .IsRequired()
            .HasMaxLength(500);

        // Configures the AgreedPrice property for the GuideSession entity.
        builder.Property(x => x.AgreedPrice)
            .IsRequired()
            .HasPrecision(18, 2);
        
        // Configures the Currency property for the GuideSession entity.
        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);

        // Confdigure the index for the GuideRequestId property to ensure uniqueness, establishing a one-to-one relationship with GuideRequest.
        builder.HasIndex(x => x.GuideRequestId)
            .IsUnique();
    }
}