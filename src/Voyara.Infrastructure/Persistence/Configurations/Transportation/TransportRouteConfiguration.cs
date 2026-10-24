using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Transportation;

namespace Voyara.Infrastructure.Persistence.Configurations.Transportation;

/// <summary>
/// Mapping for <see cref="TransportRoute"/>, an entity owned by the CityTransport aggregate.
/// It is applied from <see cref="CityTransportConfiguration"/>.
/// </summary>
internal static class TransportRouteConfiguration
{
    /// <summary>Configures the owned route table.</summary>
    public static void Configure(OwnedNavigationBuilder<CityTransport, TransportRoute> builder)
    {
        builder.ToTable("transport_routes");
        builder.WithOwner().HasForeignKey("CityTransportId");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.Name).HasMaxLength(TransportRoute.MaxNameLength).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(TransportRoute.MaxDescriptionLength).IsRequired();
        builder.Property(r => r.Fare).HasPrecision(18, 2);

        builder.OwnsMany(r => r.Stations, StationConfiguration.Configure);
        builder.Navigation(r => r.Stations).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
