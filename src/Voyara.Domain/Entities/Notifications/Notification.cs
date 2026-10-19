using Voyara.Domain.Enums;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Entities;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Notifications;

/// <summary>
/// A message delivered to one user through one channel. Its text is a snapshot: once created, the title and
/// body never change, even if the template is edited later. It is an entity with its own lifecycle (unread,
/// then read) but raises no domain events: this module consumes events from other modules and does not
/// announce anything itself. <c>CreatedAt</c> comes from the audit base class.
/// </summary>
public sealed class Notification : AuditableEntity
{
    /// <summary>Identifier of the recipient (reference to the Identity module).</summary>
    public Guid UserId { get; private set; }

    /// <summary>The kind of event this notification is about.</summary>
    public NotificationType Type { get; private set; }

    /// <summary>The short headline.</summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>The full message text.</summary>
    public string Body { get; private set; } = string.Empty;

    /// <summary>How the notification is delivered.</summary>
    public NotificationChannel Channel { get; private set; }

    /// <summary>Whether the user has opened the notification.</summary>
    public bool IsRead { get; private set; }

    /// <summary>Optional structured data, for example the id of the related review; null when there is none.</summary>
    public NotificationPayload? Payload { get; private set; }

    private Notification()
    {
    }

    private Notification(
        Guid userId,
        NotificationType type,
        NotificationChannel channel,
        NotificationMessage message,
        NotificationPayload? payload)
    {
        UserId = userId;
        Type = type;
        Channel = channel;
        Title = message.Title;
        Body = message.Body;
        Payload = payload;
        IsRead = false;
    }

    /// <summary>Creates an unread notification after validating every rule.</summary>
    /// <param name="userId">The recipient.</param>
    /// <param name="type">The kind of event.</param>
    /// <param name="channel">The delivery channel.</param>
    /// <param name="message">The validated title and body (usually the output of a rendered template).</param>
    /// <param name="payloadJson">Optional JSON object with related data; blank means none.</param>
    /// <returns>The new notification, or the first rule that was broken.</returns>
    public static Result<Notification> Create(
        Guid userId,
        NotificationType type,
        NotificationChannel channel,
        NotificationMessage message,
        string? payloadJson = null)
    {
        if (userId == Guid.Empty)
        {
            return Result.Failure<Notification>(NotificationErrors.UserRequired);
        }

        if (!Enum.IsDefined(type))
        {
            return Result.Failure<Notification>(NotificationErrors.InvalidType);
        }

        if (!Enum.IsDefined(channel))
        {
            return Result.Failure<Notification>(NotificationErrors.InvalidChannel);
        }

        if (message is null)
        {
            return Result.Failure<Notification>(NotificationErrors.MessageRequired);
        }

        NotificationPayload? payload = null;
        if (!string.IsNullOrWhiteSpace(payloadJson))
        {
            var payloadResult = NotificationPayload.Create(payloadJson);
            if (payloadResult.IsFailure)
            {
                return Result.Failure<Notification>(payloadResult.Error);
            }

            payload = payloadResult.Value;
        }

        return new Notification(userId, type, channel, message, payload);
    }

    /// <summary>Marks the notification as read. It cannot fail and is safe to call more than once.</summary>
    public void MarkRead() => IsRead = true;
}