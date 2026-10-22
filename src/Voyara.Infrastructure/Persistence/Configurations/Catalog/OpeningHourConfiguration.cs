using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Catalog;

namespace Voyara.Infrastructure.Persistence.Configurations.Catalog
{
    /// <summary>
    /// Configuration for the OpeningHour entity type.
    /// The relationship with Destination is configured in DestinationConfiguration.
    /// </summary>
    public sealed class OpeningHourConfiguration : IEntityTypeConfiguration<OpeningHour>
    {
        /// <summary>
        /// Configures the entity type for the OpeningHour model using the specified builder.
        /// </summary>
        /// <remarks>This method sets up primary keys, property constraints, relationships, and indexes for the
        /// OpeningHour entity. It is typically called by the Entity Framework infrastructure when building the
        /// model.</remarks>
        /// <param name="builder">The builder used to configure the OpeningHour entity type and its properties.</param>
        public void Configure(EntityTypeBuilder<OpeningHour> builder)
        {
            // Configure the table name and the primary key for the OpeningHour entity.
            builder.ToTable("opening_hours");
            builder.HasKey(h => h.Id);

            // Configure the Day property; the day of week is stored as a small integer.
            builder.Property(h => h.Day).HasConversion<short>().IsRequired();

            // Configure the OpensAt and ClosesAt properties for the OpeningHour entity.
            builder.Property(h => h.OpensAt).HasColumnType("time").IsRequired();
            builder.Property(h => h.ClosesAt).HasColumnType("time").IsRequired();

            // Configure the IsClosed property for the OpeningHour entity.
            builder.Property(h => h.IsClosed).IsRequired();

            // One row per destination and day of week.
            builder.HasIndex(h => new { h.DestinationId, h.Day }).IsUnique();
        }
    }
}
