using Voyara.Domain.Entities.Notifications;

namespace Voyara.Domain.Repositories.Notifications;

/// <summary>
/// Represents the repository contract for managing <see cref="Notification"/> aggregate persistence operations.
/// </summary>
/// <remarks>
/// Defines asynchronous retrieval methods for fetching notifications by identifier or user association, 
/// as well as command operations for adding new notification entities to the persistence context.
/// </remarks>
public interface INotificationRepository
{
    /// <summary>
    /// Retrieves a specific notification by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the notification.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the <see cref="Notification"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Notification?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all notifications associated with a specific user asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose notifications are being retrieved.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a read-only list of notifications belonging to the user.
    /// </returns>
    Task<IReadOnlyList<Notification>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all unread notifications associated with a specific user asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose unread notifications are being retrieved.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a read-only list of unread notifications belonging to the user.
    /// </returns>
    Task<IReadOnlyList<Notification>> GetUnreadByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new notification entity to the repository tracking context.
    /// </summary>
    /// <param name="notification">The <see cref="Notification"/> instance to add.</param>
    /// <remarks>
    /// This method stages the entity for insertion into the database. Persistence changes must be committed via the unit of work.
    /// </remarks>
    void Add(Notification notification);
}