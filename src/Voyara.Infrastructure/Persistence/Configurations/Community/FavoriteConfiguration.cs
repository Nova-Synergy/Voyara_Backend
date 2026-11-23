using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Community;

namespace Voyara.Infrastructure.Persistence.Configurations.Community;

/// <summary>
/// Configures the Favorite aggregate root for Entity Framework Core persistence.
/// </summary>
/// <remarks>
/// <para>
/// This configuration establishes the database schema and constraints for the Favorite entity,
/// which represents destinations saved by users. Favorite is an aggregate root with a soft-delete
/// flag and a unique constraint on the (UserId, DestinationId) pair to prevent duplicates.
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// Table mapping to "community_favorites" with required properties for UserId and DestinationId.
/// </description>
/// </item>
/// <item>
/// <description>
/// Unique composite index on (UserId, DestinationId) to enforce identity rules and prevent
/// duplicate records for the same user-destination pair.
/// </description>
/// </item>
/// <item>
/// <description>
/// Global query filter to exclude soft-deleted favorites from normal queries.
/// </description>
/// </item>
/// </list>
/// </remarks>
public sealed class FavoriteConfiguration
    : IEntityTypeConfiguration<Favorite>
{
    /// <summary>
    /// Configures the Favorite entity type for the database context.
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
    /// Required constraints on UserId and DestinationId, establishing the identity of the favorite.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Unique constraint on the (UserId, DestinationId) pair to prevent duplicate records.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Global query filter to automatically exclude soft-deleted favorites from queries.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="builder">The entity type builder used to configure the Favorite entity.</param>
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("community_favorites");

        builder.HasKey(favorite => favorite.Id);

        builder.Property(favorite => favorite.UserId)
            .IsRequired();

        builder.Property(favorite => favorite.DestinationId)
            .IsRequired();

        // Prevent duplicate records for the same user and destination.
        // The index includes soft deleted rows, so restoring a favorite
        // can reuse its existing record.
        builder.HasIndex(favorite => new
        {
            favorite.UserId,
            favorite.DestinationId
        }).IsUnique();

        builder.HasQueryFilter(favorite => !favorite.IsDeleted);
    }
}
