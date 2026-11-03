
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Notifications;
using Voyara.Domain.ValueObjects;

namespace Voyara.Infrastructure.Persistence.Configurations.Notifications;

/// <summary>
/// Represents the Entity Framework Core fluent API configuration for the <see cref="Notification"/> aggregate entity.
/// </summary>
/// <remarks>
/// Configures the relational database schema mapping, including primary keys, string length constraints, 
/// PostgreSQL-specific JSONB conversions for payload data, composite indexes for high-throughput query optimization, 
/// and global query filters for soft-deletion enforcement.
/// </remarks>
public sealed class NotificationConfiguration
    : IEntityTypeConfiguration<Notification>
{
    /// <summary>
    /// Configures the entity schema, table mappings, column types, and constraints for the <see cref="Notification"/> type.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="EntityTypeBuilder{TEntity}"/> instance used to configure the <see cref="Notification"/> entity properties and relationships.
    /// </param>
    /// <remarks>
    /// This configuration establishes the following persistence rules:
    /// <list type="bullet">
    /// <item><description>Maps the entity to the <c>notifications</c> database table.</description></item>
    /// <item><description>Configures string length constraints derived from domain validation constants.</description></item>
    /// <item><description>Serializes and deserializes <see cref="NotificationPayload"/> objects into PostgreSQL <c>jsonb</c> column storage via custom value conversion.</description></item>
    /// <item><description>Applies compound indexes optimized for user notification retrieval and status filtering workflows.</description></item>
    /// <item><description>Enforces a global query filter to exclude soft-deleted records (<c>IsDeleted == true</c>) from ordinary queries.</description></item>
    /// </list>
    /// </remarks>
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(notification => notification.Id);

        builder.Property(notification => notification.UserId)
            .IsRequired();

        builder.Property(notification => notification.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(notification => notification.Channel)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(notification => notification.Title)
            .HasMaxLength(NotificationMessage.MaxTitleLength)
            .IsRequired();

        builder.Property(notification => notification.Body)
            .HasMaxLength(NotificationMessage.MaxBodyLength)
            .IsRequired();

        builder.Property(notification => notification.IsRead)
            .IsRequired();

        // Store the optional NotificationPayload as PostgreSQL JSONB.
        builder.Property(notification => notification.Payload)
            .HasConversion(
                payload => payload == null ? null : payload.Json,
                json => json == null
                    ? null
                    : NotificationPayload.Create(json).Value)
            .HasColumnName("payload_json")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(notification => notification.CreatedBy)
            .HasMaxLength(100);

        builder.Property(notification => notification.UpdatedBy)
            .HasMaxLength(100);

        builder.HasIndex(notification => notification.UserId);

        builder.HasIndex(notification => new
        {
            notification.UserId,
            notification.IsRead
        });

        builder.HasIndex(notification => notification.CreatedAt);

        builder.HasIndex(notification => new
        {
            notification.UserId,
            notification.Type,
            notification.Channel
        });

        builder.HasQueryFilter(notification => !notification.IsDeleted);
    }
}
