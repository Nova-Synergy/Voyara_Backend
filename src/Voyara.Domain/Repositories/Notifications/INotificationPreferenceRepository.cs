using Voyara.Domain.Entities.Notifications;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Repositories.Notifications;

/// <summary>
/// Represents the repository contract for managing <see cref="NotificationPreference"/> persistence operations.
/// </summary>
/// <remarks>
/// Defines asynchronous retrieval methods for fetching user-specific delivery preferences by identifier, 
/// notification type, and channel, as well as command operations for adding new preference configurations.
/// </remarks>
public interface INotificationPreferenceRepository
{
    /// <summary>
    /// Retrieves a specific notification preference matching the given user, notification type, and delivery channel asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="type">The notification type associated with the preference.</param>
    /// <param name="channel">The delivery channel associated with the preference.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the matching <see cref="NotificationPreference"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<NotificationPreference?> GetByUserTypeAndChannelAsync(
        Guid userId,
        NotificationType type,
        NotificationChannel channel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all notification preferences associated with a specific user asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose preferences are being retrieved.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a read-only list of preferences belonging to the user.
    /// </returns>
    Task<IReadOnlyList<NotificationPreference>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new notification preference entity to the repository tracking context.
    /// </summary>
    /// <param name="preference">The <see cref="NotificationPreference"/> instance to add.</param>
    /// <remarks>
    /// This method stages the entity for insertion into the database. Persistence changes must be committed via the unit of work.
    /// </remarks>
    void Add(NotificationPreference preference);
}