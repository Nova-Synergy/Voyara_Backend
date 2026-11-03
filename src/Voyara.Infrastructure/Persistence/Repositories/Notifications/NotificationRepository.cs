using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Notifications;
using Voyara.Domain.Repositories.Notifications;

namespace Voyara.Infrastructure.Persistence.Repositories.Notifications;

/// <summary>
/// Provides the Entity Framework Core implementation of the <see cref="INotificationRepository"/> interface.
/// </summary>
/// <remarks>
/// Implements asynchronous data access queries and command staging for <see cref="Notification"/> aggregates, 
/// leveraging query tracking behaviors (such as <c>AsNoTracking</c>) and optimized ordering for user notification streams.
/// </remarks>
public sealed class NotificationRepository : INotificationRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for persistence operations.</param>
    public NotificationRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<Notification?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Notifications
            .FirstOrDefaultAsync(
                notification => notification.Id == id,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(notification => notification.UserId == userId)
            .OrderByDescending(notification => notification.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Notification>> GetUnreadByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(notification =>
                notification.UserId == userId &&
                !notification.IsRead)
            .OrderByDescending(notification => notification.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Add(Notification notification)
    {
        _context.Notifications.Add(notification);
    }
}