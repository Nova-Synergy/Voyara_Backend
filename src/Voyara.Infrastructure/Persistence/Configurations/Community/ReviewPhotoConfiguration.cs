using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Community;

namespace Voyara.Infrastructure.Persistence.Configurations.Community;

/// <summary>
/// Configures the ReviewPhoto owned entity for Entity Framework Core persistence.
/// </summary>
/// <remarks>
/// <para>
/// This configuration establishes the database schema and constraints for the ReviewPhoto entity,
/// which represents photos attached to a Review aggregate root. ReviewPhoto is an owned entity
/// that cannot exist independently of its parent Review.
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// Table mapping to "community_review_photos" with required properties for ReviewId and Url.
/// </description>
/// </item>
/// <item>
/// <description>
/// Index on ReviewId for efficient queries filtering photos by their parent review.
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
/// </list>
/// </remarks>
public sealed class ReviewPhotoConfiguration
    : IEntityTypeConfiguration<ReviewPhoto>
{
    /// <summary>
    /// Configures the ReviewPhoto entity type for the database context.
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
    /// Required constraint on ReviewId, establishing the foreign key relationship to the
    /// owning Review aggregate.
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
    /// Database index on ReviewId to optimize queries that retrieve all photos for a
    /// specific review.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="builder">The entity type builder used to configure the ReviewPhoto entity.</param>
    public void Configure(EntityTypeBuilder<ReviewPhoto> builder)
    {
        builder.ToTable("community_review_photos");

        builder.HasKey(photo => photo.Id);

        builder.Property(photo => photo.ReviewId)
            .IsRequired();

        builder.Property(photo => photo.Url)
            .HasMaxLength(2048)
            .IsRequired();

        builder.HasIndex(photo => photo.ReviewId);
    }
}
