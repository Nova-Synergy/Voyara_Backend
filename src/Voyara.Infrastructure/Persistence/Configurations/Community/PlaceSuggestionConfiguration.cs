using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using Voyara.Domain.Entities.Community;
using Voyara.Domain.ValueObjects;

namespace Voyara.Infrastructure.Persistence.Configurations.Community;

/// <summary>
/// Configures the PlaceSuggestion aggregate root and its owned photos for Entity Framework Core persistence.
/// </summary>
/// <remarks>
/// <para>
/// This configuration establishes the database schema and constraints for the PlaceSuggestion entity,
/// which represents tourist proposals for new tourism places. It includes soft-delete filtering,
/// geographic coordinate storage, and a one-to-many relationship with owned SuggestionPhoto entities.
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// Table mapping to "community_place_suggestions" with required properties for UserId, Name, Description, Reason, and Status.
/// </description>
/// </item>
/// <item>
/// <description>
/// GeoPoint storage as a PostGIS point using WGS84 coordinates with SRID 4326.
/// </description>
/// </item>
/// <item>
/// <description>
/// Indexes on UserId, Status, and DestinationId for efficient querying and filtering.
/// </description>
/// </item>
/// <item>
/// <description>
/// Global query filter to exclude soft-deleted PlaceSuggestion records.
/// </description>
/// </item>
/// <item>
/// <description>
/// One-to-many relationship with SuggestionPhoto entities, using the private _photos field
/// for entity loading to maintain aggregate encapsulation.
/// </description>
/// </item>
/// </list>
/// </remarks>
public sealed class PlaceSuggestionConfiguration
    : IEntityTypeConfiguration<PlaceSuggestion>
{
    /// <summary>
    /// Configures the PlaceSuggestion entity type for the database context.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is called during DbContext model creation to establish:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// Primary key on the Id property.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Required constraints on UserId, Name, Description, Reason, and Status.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// GeoPoint conversion to PostGIS point with WGS84 coordinates and SRID 4326.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Indexes on UserId, Status, and DestinationId for query optimization.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Global query filter to automatically exclude soft-deleted records.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Cascade delete behavior for owned SuggestionPhoto entities.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Field-based access for the _photos collection to preserve aggregate encapsulation.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="builder">The entity type builder used to configure the PlaceSuggestion entity.</param>
    public void Configure(EntityTypeBuilder<PlaceSuggestion> builder)
    {
        builder.ToTable("community_place_suggestions");

        builder.HasKey(suggestion => suggestion.Id);

        builder.Property(suggestion => suggestion.UserId)
            .IsRequired();

        builder.Property(suggestion => suggestion.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(suggestion => suggestion.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(suggestion => suggestion.Reason)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(suggestion => suggestion.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(suggestion => suggestion.DecisionReason)
            .HasMaxLength(1000);

        // Store GeoPoint as a PostGIS point using WGS84 coordinates.
        builder.Property(suggestion => suggestion.Location)
            .HasConversion(
                location => new Point(
                    location.Longitude,
                    location.Latitude)
                {
                    SRID = 4326
                },
                point => GeoPoint.Create(
                    point.Y,
                    point.X).Value)
            .HasColumnType("geometry(Point,4326)")
            .IsRequired();

        builder.HasIndex(suggestion => suggestion.UserId);
        builder.HasIndex(suggestion => suggestion.Status);
        builder.HasIndex(suggestion => suggestion.DestinationId);

        builder.HasQueryFilter(suggestion => !suggestion.IsDeleted);

        // A suggestion owns its photos.
        builder.HasMany(suggestion => suggestion.Photos)
            .WithOne()
            .HasForeignKey(photo => photo.PlaceSuggestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(suggestion => suggestion.Photos)
            .HasField("_photos")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
