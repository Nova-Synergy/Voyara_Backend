using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Notifications;

/// <summary>Expected business errors shared by the notification entities.</summary>
public static class NotificationErrors
{
    /// <summary>The recipient or owning user is missing.</summary>
    public static readonly Error UserRequired =
        Error.Validation("Notification.UserRequired", "The user is required.");

    /// <summary>The notification type is not a defined value.</summary>
    public static readonly Error InvalidType =
        Error.Validation("Notification.InvalidType", "The notification type is invalid.");

    /// <summary>The delivery channel is not a defined value.</summary>
    public static readonly Error InvalidChannel =
        Error.Validation("Notification.InvalidChannel", "The notification channel is invalid.");

    /// <summary>The message is missing.</summary>
    public static readonly Error MessageRequired =
        Error.Validation("Notification.MessageRequired", "The notification message is required.");
}