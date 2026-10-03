using Microsoft.Extensions.Logging;
using Voyara.Application.Abstractions;

namespace Voyara.Infrastructure.Notifications;

/// <summary>Email notification channel stub; no SMTP provider is integrated yet.</summary>
public sealed class EmailNotificationChannel : INotificationChannel
{
    private readonly ILogger<EmailNotificationChannel> _logger;

    /// <summary>Creates the channel.</summary>
    public EmailNotificationChannel(ILogger<EmailNotificationChannel> logger) => _logger = logger;

    /// <inheritdoc />
    public string Channel => "Email";

    /// <inheritdoc />
    public Task SendAsync(string recipient, string title, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[Email] To {Recipient}: {Title} - {Body}", recipient, title, body);
        return Task.CompletedTask;
    }
}
