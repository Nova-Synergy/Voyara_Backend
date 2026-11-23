using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Community;

namespace Voyara.Infrastructure.Persistence.Configurations.Community;

/// <summary>
/// Configures the SuggestionPhoto owned entity for Entity Framework Core persistence.
/// </summary>
/// <remarks>
/// <para>
/// This configuration establishes the database schema and constraints for the SuggestionPhoto entity,
/// which represents photos attached to a PlaceSuggestion aggregate root. SuggestionPhoto is an
/// owned entity that cannot exist independently of its parent PlaceSuggestion.
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// Table mapping to "community_suggestion_photos" with required properties for PlaceSuggestionId and Url.
/// </description>
/// </item>
/// <item>
/// <description>
/// Index on PlaceSuggestionId for efficient queries filtering photos by their parent suggestion.
/// </description>
/// </item>
/// <item>
/// <description>
/// Primary key on the Id property, inherited from BaseEntity.
/// </description>
/// </item>
/// <item>
/// <description>
/// Maximum URL length constraint of 2048 characters to support long image URLs.
/// </description>
/// </item>
/// <item>
/// <description>
/// Global query filter to exclude soft-deleted photos from queries.
/// </description>
/// </item>
/// </list>
/// </remarks>
public sealed class SuggestionPhotoConfiguration
    : IEntityTypeConfiguration<SuggestionPhoto>
{
    /// <summary>
    /// Configures the SuggestionPhoto entity type for the database context.
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
    /// Required constraint on PlaceSuggestionId, establishing the foreign key relationship to the
    /// owning PlaceSuggestion aggregate.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Required constraint on Url with a maximum length of 2048 characters to accommodate
    /// typical cloud storage URLs.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Database index on PlaceSuggestionId to optimize queries that retrieve all photos for a
    /// specific suggestion.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Global query filter to automatically exclude soft-deleted photos from queries.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="builder">The entity type builder used to configure the SuggestionPhoto entity.</param>
    public void Configure(EntityTypeBuilder<SuggestionPhoto> builder)
    {
        builder.ToTable("community_suggestion_photos");

        builder.HasKey(photo => photo.Id);

        builder.Property(photo => photo.PlaceSuggestionId)
            .IsRequired();

        builder.Property(photo => photo.Url)
            .HasMaxLength(2048)
            .IsRequired();

        builder.HasIndex(photo => photo.PlaceSuggestionId);

        builder.HasQueryFilter(photo => !photo.IsDeleted);
    }
}
