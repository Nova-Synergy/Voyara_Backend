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
    /// EF Core Fluent API mapping for <see cref="TransportRoute"/>.
    /// </summary>
    public class TransportRouteConfiguration : IEntityTypeConfiguration<TransportRoute>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<TransportRoute> builder)
        {
            builder.ToTable("TransportRoutes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.Fare).HasPrecision(18, 2);   // decimal? becomes a nullable column

            // TransportRoute 1 --- 2..* Station.
            // The minimum of two stations is a business rule and must be validated in the application layer.
            builder.HasMany(x => x.Stations)
                   .WithOne(s => s.TransportRoute)
                   .HasForeignKey(s => s.TransportRouteId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
