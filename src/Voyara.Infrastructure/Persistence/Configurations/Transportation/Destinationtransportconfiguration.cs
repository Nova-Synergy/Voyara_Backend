using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Entities.Transportation;

namespace Voyara.Infrastructure.Persistence.Configurations.Transportation;

/// <summary>
/// Mapping for <see cref="DestinationTransport"/>, a value object owned by the CityTransport aggregate.
/// It is applied from <see cref="CityTransportConfiguration"/>.
/// </summary>
internal static class DestinationTransportConfiguration
{
    /// <summary>Configures the owned destination link table.</summary>
    public static void Configure(OwnedNavigationBuilder<CityTransport, DestinationTransport> builder)
    {
        builder.ToTable("destination_transports");
        builder.WithOwner().HasForeignKey("CityTransportId");

        // A destination can be linked to a city transport only once.
        builder.HasKey("CityTransportId", nameof(DestinationTransport.DestinationId));
        builder.Property(d => d.DestinationId).ValueGeneratedNever();

        // Reference to the Destination aggregate by Id (foreign key, no navigation property).
        builder.HasOne<Destination>()
            .WithMany()
            .HasForeignKey(d => d.DestinationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Supports "which transports reach this destination" queries.
        builder.HasIndex(d => d.DestinationId);
    }
}
