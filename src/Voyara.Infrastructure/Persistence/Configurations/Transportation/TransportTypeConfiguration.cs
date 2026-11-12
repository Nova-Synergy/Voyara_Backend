using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Transportation;

namespace Voyara.Infrastructure.Persistence.Configurations.Transportation;

/// <summary>
/// Mapping for the TransportType reference data: fixed identifiers, unique code and seed data.
/// </summary>
public sealed class TransportTypeConfiguration : IEntityTypeConfiguration<TransportType>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TransportType> builder)
    {
        builder.ToTable("transport_types");
        builder.HasKey(t => t.Id);

        // Fixed reference data with explicitly assigned identifiers (no identity column).
        builder.Property(t => t.Id).ValueGeneratedNever();

        builder.Property(t => t.Code).HasMaxLength(TransportType.MaxCodeLength).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(TransportType.MaxNameLength).IsRequired();

        builder.HasIndex(t => t.Code).IsUnique();

        builder.HasData(
            new { Id = 1, Code = "METRO", Name = "Metro" },
            new { Id = 2, Code = "BUS", Name = "Bus" },
            new { Id = 3, Code = "MICROBUS", Name = "Microbus" },
            new { Id = 4, Code = "TAXI", Name = "Taxi" },
            new { Id = 5, Code = "RIDE_HAILING", Name = "Ride-hailing" },
            new { Id = 6, Code = "TRAM", Name = "Tram" });
    }
}
