using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Catalog;

namespace Voyara.Infrastructure.Persistence.Configurations.Catalog
{
    /// <summary>
    /// Configuration for the OpeningException entity type.
    /// The relationship with Destination is configured in DestinationConfiguration.
    /// </summary>
    public sealed class OpeningExceptionConfiguration : IEntityTypeConfiguration<OpeningException>
    {
        /// <summary>
        /// Configures the entity type for the OpeningException model using the specified builder.
        /// </summary>
        /// <remarks>This method sets up primary keys, property constraints, relationships, and indexes for the
        /// OpeningException entity. It is typically called by the Entity Framework infrastructure when building the
        /// model.</remarks>
        /// <param name="builder">The builder used to configure the OpeningException entity type and its properties.</param>
        public void Configure(EntityTypeBuilder<OpeningException> builder)
        {
            // Configure the table name and the primary key for the OpeningException entity.
            builder.ToTable("opening_exceptions");
            builder.HasKey(e => e.Id);

            // Configure the Date property for the OpeningException entity.
            builder.Property(e => e.Date).HasColumnType("date").IsRequired();

            // Configure the IsClosed property for the OpeningException entity.
            builder.Property(e => e.IsClosed).IsRequired();

            // Configure the Reason property for the OpeningException entity.
            builder.Property(e => e.Reason).HasMaxLength(500).IsRequired();

            // One exception per destination and date.
            builder.HasIndex(e => new { e.DestinationId, e.Date }).IsUnique();
        }
    }
}
