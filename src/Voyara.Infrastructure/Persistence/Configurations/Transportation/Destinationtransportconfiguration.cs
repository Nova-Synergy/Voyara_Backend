using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Entities.Transportation;

namespace Voyara.Infrastructure.Persistence.Configurations.Transportation
{

    /// <summary>
    /// EF Core Fluent API mapping for the <see cref="DestinationTransport"/> join entity.
    /// </summary>
    /// <remarks>
    /// Links <see cref="Destination"/> (Catalog) to <see cref="CityTransport"/>. The foreign key to the destination
    /// has no navigation property, so the Catalog aggregate does not depend on Transportation.
    /// </remarks>
    public sealed class DestinationTransportConfiguration : IEntityTypeConfiguration<DestinationTransport>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<DestinationTransport> builder)
        {
            builder.ToTable("destination_transports");

            // Composite key prevents linking the same destination to the same transport twice.
            // Its leading column (DestinationId) also serves "which transports reach this destination" queries,
            // so no extra index is needed.
            builder.HasKey(x => new { x.DestinationId, x.CityTransportId });

            builder.HasOne(x => x.CityTransport)
                   .WithMany(c => c.Destinations)
                   .HasForeignKey(x => x.CityTransportId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Destination>()
                   .WithMany()
                   .HasForeignKey(x => x.DestinationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
