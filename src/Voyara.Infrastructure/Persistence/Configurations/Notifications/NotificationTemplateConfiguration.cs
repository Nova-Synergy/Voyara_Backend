using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Notifications;
using Voyara.Domain.ValueObjects;

namespace Voyara.Infrastructure.Persistence.Configurations.Notifications;

/// <summary>
/// Represents the Entity Framework Core fluent API configuration for the <see cref="NotificationTemplate"/> entity.
/// </summary>
/// <remarks>
/// Configures the database schema mapping for reusable and translated notification templates, including table mapping,
/// custom value conversions for the <see cref="LanguageCode"/> value object, string length constraints, unique composite indexes
/// ensuring exactly one template per type and language combination, and global query filters for soft-deletion handling.
/// </remarks>
public sealed class NotificationTemplateConfiguration
    : IEntityTypeConfiguration<NotificationTemplate>
{
    /// <summary>
    /// Configures the entity schema, table mappings, value conversions, constraints, unique indexes, and global query filters for <see cref="NotificationTemplate"/>.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="EntityTypeBuilder{TEntity}"/> instance used to configure the <see cref="NotificationTemplate"/> entity properties and indices.
    /// </param>
    /// <remarks>
    /// This configuration establishes the following persistence rules:
    /// <list type="bullet">
    /// <item><description>Maps the entity to the <c>notification_templates</c> database table.</description></item>
    /// <item><description>Converts notification type enums to string representations with explicit maximum lengths.</description></item>
    /// <item><description>Applies a custom value converter for the <see cref="LanguageCode"/> value object to map its underlying string representation to a <c>language_code</c> column with a maximum length of 5.</description></item>
    /// <item><description>Configures subject and body length constraints derived from domain validation constants.</description></item>
    /// <item><description>Establishes a unique composite index across <c>Type</c> and <c>LanguageCode</c> to enforce uniqueness per notification type and localization.</description></item>
    /// <item><description>Applies a global query filter to automatically omit soft-deleted template records (<c>IsDeleted == true</c>) from ordinary queries.</description></item>
    /// </list>
    /// </remarks>
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("notification_templates");

        builder.HasKey(template => template.Id);

        builder.Property(template => template.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(template => template.LanguageCode)
            .HasConversion(
                language => language.Value,
                value => LanguageCode.Create(value).Value)
            .HasColumnName("language_code")
            .HasMaxLength(5)
            .IsRequired();

        builder.Property(template => template.Subject)
            .HasMaxLength(NotificationMessage.MaxTitleLength)
            .IsRequired();

        builder.Property(template => template.Body)
            .HasMaxLength(NotificationMessage.MaxBodyLength)
            .IsRequired();

        builder.Property(template => template.CreatedBy)
            .HasMaxLength(100);

        builder.Property(template => template.UpdatedBy)
            .HasMaxLength(100);

        // Exactly one template for each notification type and language.
        builder.HasIndex(template => new
        {
            template.Type,
            template.LanguageCode
        }).IsUnique();

        builder.HasQueryFilter(template => !template.IsDeleted);
    }
}