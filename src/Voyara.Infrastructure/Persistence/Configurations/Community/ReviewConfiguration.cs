using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Community;

namespace Voyara.Infrastructure.Persistence.Configurations.Community;

/// <summary>
/// Configures the Review aggregate root for Entity Framework Core persistence.
/// </summary>
/// <remarks>
/// <para>
/// This configuration establishes the database schema and constraints for the Review entity,
/// including:
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// Table mapping to "community_reviews" with required properties for UserId, DestinationId,
/// Rating, Text, VisitDate, and Status.
/// </description>
/// </item>
/// <item>
/// <description>
/// Indexes on UserId, DestinationId, and Status for efficient query filtering,
/// plus a composite index on (UserId, DestinationId) for review uniqueness checks.
/// </description>
/// </item>
/// <item>
/// <description>
/// Global query filter that excludes soft-deleted reviews from all queries by default.
/// </description>
/// </item>
/// <item>
/// <description>
/// Cascade delete behavior for owned ReviewPhoto entities, ensuring photos are removed
/// when their parent review is deleted.
/// </description>
/// </item>
/// <item>
/// <description>
/// Field-based access for the _photos collection to preserve the Review aggregate's
/// encapsulation over its photo collection.
/// </description>
/// </item>
/// </list>
/// </remarks>
public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    /// <summary>
    /// Configures the Review entity type for the database context.
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
    /// Required constraints on UserId, DestinationId, Rating, Text, VisitDate,
    /// and Status, with appropriate max lengths for string properties.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Enum-to-string conversion for the Status property, stored as a string with
    /// max length of 20 characters.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Optional RejectionReason field with a maximum length of 1000 characters,
    /// populated only when a review is rejected.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Database indexes for common filtering patterns (by user, destination, status,
    /// and the composite user-destination pair).
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Query filter to automatically exclude soft-deleted reviews from queries.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// One-to-many relationship with ReviewPhoto entities, using the private _photos
    /// field for entity loading to maintain aggregate encapsulation.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="builder">The entity type builder used to configure the Review entity.</param>
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("community_reviews");

        builder.HasKey(review => review.Id);

        builder.Property(review => review.UserId)
            .IsRequired();

        builder.Property(review => review.DestinationId)
            .IsRequired();

        builder.Property(review => review.Rating)
            .IsRequired();

        builder.Property(review => review.Text)
            .HasMaxLength(3000)
            .IsRequired();

        builder.Property(review => review.VisitDate)
            .IsRequired();

        builder.Property(review => review.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(review => review.RejectionReason)
            .HasMaxLength(1000);

        builder.HasIndex(review => review.UserId);
        builder.HasIndex(review => review.DestinationId);
        builder.HasIndex(review => review.Status);

        builder.HasIndex(review => new
        {
            review.UserId,
            review.DestinationId
        });

        // Exclude soft deleted reviews from normal queries.
        builder.HasQueryFilter(review => !review.IsDeleted);

        // A review owns its photos.
        builder.HasMany(review => review.Photos)
            .WithOne()
            .HasForeignKey(photo => photo.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        // Use the private collection field when EF loads photos.
        builder.Navigation(review => review.Photos)
            .HasField("_photos")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
