using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Notifications;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Notifications;

namespace Voyara.Infrastructure.Persistence.Repositories.Notifications;

/// <summary>
/// Provides the Entity Framework Core implementation of the <see cref="INotificationPreferenceRepository"/> interface.
/// </summary>
/// <remarks>
/// Implements asynchronous queries for retrieving user notification preferences by specific criteria or in bulk, 
/// utilizing tracking exclusions and deterministic ordering by type and channel.
/// </remarks>
public sealed class NotificationPreferenceRepository
    : INotificationPreferenceRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationPreferenceRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for persistence operations.</param>
    public NotificationPreferenceRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<NotificationPreference?> GetByUserTypeAndChannelAsync(
        Guid userId,
        NotificationType type,
        NotificationChannel channel,
        CancellationToken cancellationToken = default)
    {
        return _context.NotificationPreferences
            .FirstOrDefaultAsync(
                preference =>
                    preference.UserId == userId &&
                    preference.Type == type &&
                    preference.Channel == channel,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<NotificationPreference>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.NotificationPreferences
            .AsNoTracking()
            .Where(preference => preference.UserId == userId)
            .OrderBy(preference => preference.Type)
            .ThenBy(preference => preference.Channel)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Add(NotificationPreference preference)
    {
        _context.NotificationPreferences.Add(preference);
    }
}