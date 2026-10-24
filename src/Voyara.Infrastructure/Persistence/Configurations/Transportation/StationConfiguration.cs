using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using Voyara.Domain.Entities.Transportation;
using Voyara.Domain.ValueObjects;

namespace Voyara.Infrastructure.Persistence.Configurations.Transportation;

/// <summary>
/// Mapping for <see cref="Station"/>, an entity owned by <see cref="TransportRoute"/>.
/// It is applied from <see cref="TransportRouteConfiguration"/>.
/// </summary>
internal static class StationConfiguration
{
    /// <summary>Configures the owned station table.</summary>
    public static void Configure(OwnedNavigationBuilder<TransportRoute, Station> builder)
    {
        builder.ToTable("stations");
        builder.WithOwner().HasForeignKey("TransportRouteId");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.Name).HasMaxLength(Station.MaxNameLength).IsRequired();

        // Same GeoPoint-to-Point conversion as Destination.Location.
        builder.Property(s => s.Location)
            .HasConversion(
                geoPoint => new Point(geoPoint.Longitude, geoPoint.Latitude) { SRID = 4326 },
                point => GeoPoint.Create(point.Y, point.X).Value)
            .HasColumnType("geometry(Point,4326)")
            .IsRequired();
    }
}
