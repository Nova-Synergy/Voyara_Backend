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
    /// EF Core Fluent API mapping for <see cref="CityTransport"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="CityTransport.CityId"/> references the <c>City</c> <c>«ref»</c>, which is not an entity of this
    /// model yet, so it is mapped as a plain indexed column without a foreign key constraint.
    /// </remarks>
    public sealed class CityTransportConfiguration : IEntityTypeConfiguration<CityTransport>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<CityTransport> builder)
        {
            builder.ToTable("city_transports");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.ApproxCost).HasPrecision(18, 2);
            builder.Property(x => x.Currency).IsRequired().HasMaxLength(3);   // ISO 4217
            builder.Property(x => x.OperatingHours).IsRequired().HasMaxLength(200);
            builder.Property(x => x.SafetyInfo).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.UpdatedAt).IsRequired();

            // TransportType 1 --- 0..* CityTransport.
            // Restrict: a transport type that is in use cannot be deleted.
            builder.HasOne(x => x.TransportType)
                   .WithMany(t => t.CityTransports)
                   .HasForeignKey(x => x.TransportTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            // One entry per (city, transport type). Also serves lookups by CityId.
            builder.HasIndex(x => new { x.CityId, x.TransportTypeId }).IsUnique();

            // Composition: CityTransport owns its routes, so deleting it deletes them.
            builder.HasMany(x => x.Routes)
                   .WithOne(r => r.CityTransport)
                   .HasForeignKey(r => r.CityTransportId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
