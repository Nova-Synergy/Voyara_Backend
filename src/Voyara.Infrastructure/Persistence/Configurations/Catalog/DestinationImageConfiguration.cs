using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Catalog;

namespace Voyara.Infrastructure.Persistence.Configurations.Catalog
{
    /// <summary>
    /// Configuration for the DestinationImage entity type.
    /// The relationship with Destination is configured in DestinationConfiguration.
    /// </summary>
    public sealed class DestinationImageConfiguration : IEntityTypeConfiguration<DestinationImage>
    {
        /// <summary>
        /// Configures the entity type for the DestinationImage model using the specified builder.
        /// </summary>
        /// <remarks>This method sets up primary keys, property constraints, relationships, and indexes for the
        /// DestinationImage entity. It is typically called by the Entity Framework infrastructure when building the
        /// model.</remarks>
        /// <param name="builder">The builder used to configure the DestinationImage entity type and its properties.</param>
        public void Configure(EntityTypeBuilder<DestinationImage> builder)
        {
            // Configure the table name and the primary key for the DestinationImage entity.
            builder.ToTable("destination_images");
            builder.HasKey(i => i.Id);

            // Configure the Url property for the DestinationImage entity.
            builder.Property(i => i.Url).HasMaxLength(2048).IsRequired();

            // Configure the SortOrder and IsCover properties for the DestinationImage entity.
            builder.Property(i => i.SortOrder).IsRequired();
            builder.Property(i => i.IsCover).IsRequired();

            // Loading the pictures of a destination in order.
            builder.HasIndex(i => new { i.DestinationId, i.SortOrder });

            // At most one cover picture per destination.
            builder.HasIndex(i => i.DestinationId)
                .IsUnique()
                .HasFilter("\"IsCover\" = TRUE")
                .HasDatabaseName("UX_destination_images_cover");
        }
    }
}
