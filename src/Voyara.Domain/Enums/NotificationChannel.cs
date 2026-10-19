namespace Voyara.Domain.Enums;

/// <summary>
/// The way a notification is delivered (pluggable channels, CLS-08). Names match the channel names used by
/// the delivery adapters ("InApp", "Email").
/// </summary>
public enum NotificationChannel
{
    /// <summary>Shown inside the application.</summary>
    InApp = 1,

    /// <summary>Sent by e-mail.</summary>
    Email = 2,

    /// <summary>Sent as a push notification to the user's device (planned).</summary>
    Push = 3
}