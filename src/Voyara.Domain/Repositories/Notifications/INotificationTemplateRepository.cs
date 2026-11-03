using Voyara.Domain.Entities.Notifications;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Repositories.Notifications;

/// <summary>
/// Represents the repository contract for managing <see cref="NotificationTemplate"/> persistence operations.
/// </summary>
/// <remarks>
/// Defines asynchronous retrieval methods for fetching translated notification templates by notification type and language code, 
/// as well as command operations for adding new template entities to the persistence context.
/// </remarks>
public interface INotificationTemplateRepository
{
    /// <summary>
    /// Retrieves a notification template matching the specified notification type and language code asynchronously.
    /// </summary>
    /// <param name="type">The notification type associated with the template.</param>
    /// <param name="languageCode">The language code representing the template's localization.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the matching <see cref="NotificationTemplate"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<NotificationTemplate?> GetByTypeAndLanguageAsync(
        NotificationType type,
        string languageCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new notification template entity to the repository tracking context.
    /// </summary>
    /// <param name="template">The <see cref="NotificationTemplate"/> instance to add.</param>
    /// <remarks>
    /// This method stages the entity for insertion into the database. Persistence changes must be committed via the unit of work.
    /// </remarks>
    void Add(NotificationTemplate template);
}