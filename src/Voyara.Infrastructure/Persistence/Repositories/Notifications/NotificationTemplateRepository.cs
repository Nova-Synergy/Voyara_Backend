using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Notifications;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Notifications;
using Voyara.Domain.ValueObjects;

namespace Voyara.Infrastructure.Persistence.Repositories.Notifications;

/// <summary>
/// Provides the Entity Framework Core implementation of the <see cref="INotificationTemplateRepository"/> interface.
/// </summary>
/// <remarks>
/// Implements asynchronous retrieval for translated notification templates by notification type and normalized language code, 
/// as well as command operations for staging new template entities.
/// </remarks>
public sealed class NotificationTemplateRepository
    : INotificationTemplateRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationTemplateRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for persistence operations.</param>
    public NotificationTemplateRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<NotificationTemplate?> GetByTypeAndLanguageAsync(
        NotificationType type,
        string languageCode,
        CancellationToken cancellationToken = default)
    {
        var languageResult = LanguageCode.Create(languageCode);

        if (languageResult.IsFailure)
        {
            return Task.FromResult<NotificationTemplate?>(null);
        }

        var normalizedLanguage = languageResult.Value;

        return _context.NotificationTemplates
            .FirstOrDefaultAsync(
                template =>
                    template.Type == type &&
                    template.LanguageCode == normalizedLanguage,
                cancellationToken);
    }

    /// <inheritdoc />
    public void Add(NotificationTemplate template)
    {
        _context.NotificationTemplates.Add(template);
    }
}