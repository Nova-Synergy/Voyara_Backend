using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Entities.Transportation;
using Voyara.Domain.ValueObjects;
using NetTopologySuite.Geometries;

namespace Voyara.Infrastructure.Persistence.Configurations.Transportation
{

    /// <summary>
    /// EF Core Fluent API mapping for <see cref="Station"/>, including the GeoPoint-to-Point conversion.
    /// </summary>
    /// <remarks>
    /// <see cref="Station.Location"/> is stored as a PostGIS <c>geometry(Point,4326)</c> column using the same
    /// conversion as <c>Destination.Location</c>, so spatial queries work across both tables.
    /// </remarks>
    public sealed class StationConfiguration : IEntityTypeConfiguration<Station>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Station> builder)
        {
            builder.ToTable("stations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);

            builder.Property(x => x.Location)
                .HasConversion(
                    geoPoint => new Point(geoPoint.Longitude, geoPoint.Latitude) { SRID = 4326 },
                    point => GeoPoint.Create(point.Y, point.X).Value)
                .HasColumnType("geometry(Point,4326)")
                .IsRequired();
        }
    }

}
