using Microsoft.Extensions.Logging;
using Voyara.Application.Abstractions;

namespace Voyara.Infrastructure.Notifications;

/// <summary>In-app notification channel stub; delivery integrates with SignalR/notifications module later.</summary>
public sealed class InAppNotificationChannel : INotificationChannel
{
    private readonly ILogger<InAppNotificationChannel> _logger;

    /// <summary>Creates the channel.</summary>
    public InAppNotificationChannel(ILogger<InAppNotificationChannel> logger) => _logger = logger;

    /// <inheritdoc />
    public string Channel => "InApp";

    /// <inheritdoc />
    public Task SendAsync(string recipient, string title, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[InApp] To {Recipient}: {Title} - {Body}", recipient, title, body);
        return Task.CompletedTask;
    }
}
