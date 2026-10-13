namespace Voyara.Application.Abstractions;

/// <summary>A delivery channel for user notifications (e.g. in-app, email).</summary>
public interface INotificationChannel
{
    /// <summary>Channel discriminator, e.g. "InApp" or "Email".</summary>
    string Channel { get; }

    /// <summary>Delivers a notification to a recipient.</summary>
    Task SendAsync(string recipient, string title, string body, CancellationToken cancellationToken = default);
}
