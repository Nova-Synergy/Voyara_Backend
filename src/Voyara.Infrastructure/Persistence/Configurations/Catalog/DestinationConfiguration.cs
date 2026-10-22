using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.ValueObjects;

namespace Voyara.Infrastructure.Persistence.Configurations.Catalog
{
    /// <summary>
    /// Configuration for the Destination aggregate root.
    /// Name and description live in destination_translations; the composed children (images, opening hours,
    /// opening exceptions, ticket prices and translations) are configured here as relationships.
    /// </summary>
    public sealed class DestinationConfiguration : IEntityTypeConfiguration<Destination>
    {
        /// <summary>
        /// Configures the entity type for the Destination model using the specified builder.
        /// </summary>
        /// <remarks>This method sets up the key, property constraints, soft-delete filter, PostgreSQL xmin
        /// concurrency, indexes and relationships for the Destination aggregate. It is typically called by the
        /// Entity Framework infrastructure when building the model.</remarks>
        /// <param name="builder">The builder used to configure the Destination entity type and its properties.</param>
        public void Configure(EntityTypeBuilder<Destination> builder)
        {
            // Configure the table name and the check constraints for the Destination entity.
            builder.ToTable("destinations", t =>
            {
                t.HasCheckConstraint("CK_destinations_visit_minutes", "\"EstimatedVisitMinutes\" BETWEEN 1 AND 1440");
                t.HasCheckConstraint("CK_destinations_rating", "\"AverageRating\" BETWEEN 0 AND 5");
            });

            // Configure the primary key for the Destination entity.
            builder.HasKey(d => d.Id);

            // Configure the Status property; the DestinationStatus enum is stored as text.
            builder.Property(d => d.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // Configure the numeric properties for the Destination entity.
            builder.Property(d => d.EstimatedVisitMinutes).IsRequired();
            builder.Property(d => d.AverageRating).IsRequired();
            builder.Property(d => d.ReviewCount).IsRequired();
            builder.Property(d => d.PopularityScore).IsRequired();

            // Configure the audit properties inherited from AuditableEntity.
            builder.Property(d => d.CreatedBy).HasMaxLength(100);
            builder.Property(d => d.UpdatedBy).HasMaxLength(100);

            // Configure the Location property; GeoPoint is converted to a PostGIS point.
            builder.Property(d => d.Location)
                .HasConversion(
                    geoPoint => new Point(geoPoint.Longitude, geoPoint.Latitude) { SRID = 4326 },
                    point => GeoPoint.Create(point.Y, point.X).Value)
                .HasColumnType("geometry(Point,4326)")
                .IsRequired();

            // Optimistic concurrency via the PostgreSQL xmin system column.
            builder.Property<uint>("xmin").IsRowVersion();

            // Soft delete: deleted destinations are hidden from every query.
            builder.HasQueryFilter(d => !d.IsDeleted);

            // Domain events are published through the outbox, never persisted as columns.
            builder.Ignore(d => d.DomainEvents);

            // Indexes: status/city/category filters, popularity ranking and spatial lookups.
            builder.HasIndex(d => d.Status);
            builder.HasIndex(d => new { d.CityId, d.Status });
            builder.HasIndex(d => new { d.CategoryId, d.Status });
            builder.HasIndex(d => d.PopularityScore);
            builder.HasIndex(d => d.Location).HasMethod("gist");

            // A city that has destinations cannot be hard-deleted.
            builder.HasOne(d => d.City)
                .WithMany()
                .HasForeignKey(d => d.CityId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // A category that has destinations cannot be hard-deleted.
            builder.HasOne(d => d.Category)
                .WithMany()
                .HasForeignKey(d => d.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Composition: children are owned by the destination and removed with it.
            // Pictures of the destination.
            builder.HasMany(d => d.Images)
                .WithOne(i => i.Destination)
                .HasForeignKey(i => i.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Opening hours of the destination.
            builder.HasMany(d => d.OpeningHours)
                .WithOne(h => h.Destination)
                .HasForeignKey(h => h.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Opening exceptions of the destination.
            builder.HasMany(d => d.OpeningExceptions)
                .WithOne(e => e.Destination)
                .HasForeignKey(e => e.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ticket prices of the destination.
            builder.HasMany(d => d.TicketPrices)
                .WithOne(p => p.Destination)
                .HasForeignKey(p => p.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Translations of the destination (one row per language).
            builder.HasMany(d => d.Translations)
                .WithOne(t => t.Destination)
                .HasForeignKey(t => t.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Use the private backing fields for the collection navigations.
            builder.Navigation(d => d.Images).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(d => d.OpeningHours).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(d => d.OpeningExceptions).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(d => d.TicketPrices).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(d => d.Translations).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
