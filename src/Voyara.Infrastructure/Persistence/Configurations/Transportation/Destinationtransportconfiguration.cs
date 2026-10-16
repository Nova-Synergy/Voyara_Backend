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
    /// EF Core Fluent API mapping for the <see cref="DestinationTransport"/> join entity.
    /// </summary>
    /// <remarks>
    /// <see cref="DestinationTransport.DestinationId"/> references the <c>Destination</c> <c>«ref»</c> from another
    /// module, so it is mapped as a plain indexed column without a foreign key constraint.
    /// </remarks>
    public class DestinationTransportConfiguration : IEntityTypeConfiguration<DestinationTransport>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<DestinationTransport> builder)
        {
            builder.ToTable("DestinationTransports");

            // Composite key prevents linking the same destination to the same transport twice.
            builder.HasKey(x => new { x.DestinationId, x.CityTransportId });

            builder.HasOne(x => x.CityTransport)
                   .WithMany(c => c.Destinations)
                   .HasForeignKey(x => x.CityTransportId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Supports "which transports reach this destination" queries.
            builder.HasIndex(x => x.DestinationId);
        }
    }
}
