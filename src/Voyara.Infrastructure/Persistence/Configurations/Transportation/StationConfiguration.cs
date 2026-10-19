using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Entities.Transportation;

namespace Voyara.Infrastructure.Persistence.Configurations.Transportation
{
    /// <summary>
    /// EF Core Fluent API mapping for <see cref="Station"/>.
    /// </summary>
    public class StationConfiguration : IEntityTypeConfiguration<Station>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Station> builder)
        {
            builder.ToTable("Stations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);

            // GeoPoint is a value object: map it as an owned type into the Stations table.
            builder.OwnsOne(x => x.Location, location =>
            {
                location.Property(p => p.Latitude).HasColumnName("Latitude").IsRequired();
                location.Property(p => p.Longitude).HasColumnName("Longitude").IsRequired();
            });
            builder.Navigation(x => x.Location).IsRequired();
        }
    }

}
