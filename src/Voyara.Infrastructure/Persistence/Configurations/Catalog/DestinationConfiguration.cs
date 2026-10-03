using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Enums;
using Voyara.Domain.ValueObjects;

namespace Voyara.Infrastructure.Persistence.Configurations.Catalog;

/// <summary>
/// Mapping for the Destination aggregate: status conversion, soft-delete filter,
/// PostgreSQL xmin concurrency and GeoPoint-to-Point conversion.
/// </summary>
public sealed class DestinationConfiguration : IEntityTypeConfiguration<Destination>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Destination> builder)
    {
        builder.ToTable("destinations");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name).HasMaxLength(200).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(4000);

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(d => d.Status);

        builder.HasQueryFilter(d => !d.IsDeleted);

        // Optimistic concurrency via the PostgreSQL xmin system column (reference aggregate only).
        builder.Property<uint>("xmin").IsRowVersion();

        builder.Property(d => d.Location)
            .HasConversion(
                geoPoint => new Point(geoPoint.Longitude, geoPoint.Latitude) { SRID = 4326 },
                point => GeoPoint.Create(point.Y, point.X).Value)
            .HasColumnType("geometry(Point,4326)");
    }
}
