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
    /// EF Core Fluent API mapping for <see cref="TransportType"/>, including seed data.
    /// </summary>
    public sealed class TransportTypeConfiguration : IEntityTypeConfiguration<TransportType>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<TransportType> builder)
        {
            builder.ToTable("transport_types");

            builder.HasKey(x => x.Id);

            // Lookup table with fixed, explicitly assigned Ids (no identity column), so seeding is deterministic
            // and PostgreSQL sequences never collide with the seeded rows.
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

            // Code identifies the type in code and APIs, so it must be unique.
            builder.HasIndex(x => x.Code).IsUnique();

            builder.HasData(
                new TransportType { Id = 1, Code = "METRO", Name = "Metro" },
                new TransportType { Id = 2, Code = "BUS", Name = "Bus" },
                new TransportType { Id = 3, Code = "MICROBUS", Name = "Microbus" },
                new TransportType { Id = 4, Code = "TAXI", Name = "Taxi" },
                new TransportType { Id = 5, Code = "RIDE_HAILING", Name = "Ride-hailing" },
                new TransportType { Id = 6, Code = "TRAM", Name = "Tram" });
        }
    }
}