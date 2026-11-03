using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Notifications;

namespace Voyara.Infrastructure.Persistence.Configurations.Notifications;

/// <summary>
/// Represents the Entity Framework Core fluent API configuration for the <see cref="NotificationPreference"/> entity.
/// </summary>
/// <remarks>
/// Configures the database schema mapping for user notification delivery preferences, including table naming, 
/// string length constraints for enums, unique composite indexes enforcing a single preference record per user/type/channel combination, 
/// and global query filters for soft-deletion handling.
/// </remarks>
public sealed class NotificationPreferenceConfiguration
    : IEntityTypeConfiguration<NotificationPreference>
{
    /// <summary>
    /// Configures the entity schema, table mappings, constraints, unique indexes, and global query filters for <see cref="NotificationPreference"/>.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="EntityTypeBuilder{TEntity}"/> instance used to configure the <see cref="NotificationPreference"/> entity properties and indices.
    /// </param>
    /// <remarks>
    /// This configuration establishes the following persistence rules:
    /// <list type="bullet">
    /// <item><description>Maps the entity to the <c>notification_preferences</c> database table.</description></item>
    /// <item><description>Converts notification type and delivery channel enums to string representations with explicit maximum lengths.</description></item>
    /// <item><description>Establishes a unique composite index across <c>UserId</c>, <c>Type</c>, and <c>Channel</c> to prevent duplicate configuration rows.</description></item>
    /// <item><description>Applies a global query filter to automatically omit soft-deleted preference records (<c>IsDeleted == true</c>) from queries.</description></item>
    /// </list>
    /// </remarks>
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("notification_preferences");

        builder.HasKey(preference => preference.Id);

        builder.Property(preference => preference.UserId)
            .IsRequired();

        builder.Property(preference => preference.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(preference => preference.Channel)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(preference => preference.IsEnabled)
            .IsRequired();

        builder.Property(preference => preference.CreatedBy)
            .HasMaxLength(100);

        builder.Property(preference => preference.UpdatedBy)
            .HasMaxLength(100);

        // One preference per user, notification type and channel.
        builder.HasIndex(preference => new
        {
            preference.UserId,
            preference.Type,
            preference.Channel
        }).IsUnique();

        builder.HasQueryFilter(preference => !preference.IsDeleted);
    }
}