using System.Security.Cryptography;
using System.Text;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Entities;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Notifications;

/// <summary>
/// Whether a user wants a given type of notification on a given channel (for example "e-mail me when a
/// review is approved"). The dispatcher consults these before sending (SEQ-11).
/// </summary>
/// <remarks>
/// Identity rule: the <c>Id</c> is derived from (user, type, channel) by <see cref="ComputeId"/>, matching the
/// composite primary key of the design. The same triple always has the same identity, so a user cannot hold two
/// settings for it.
/// </remarks>
public sealed class NotificationPreference : AuditableEntity
{
    /// <summary>Identifier of the user who owns the preference (reference to the Identity module).</summary>
    public Guid UserId { get; private set; }

    /// <summary>The kind of notification this preference controls.</summary>
    public NotificationType Type { get; private set; }

    /// <summary>The delivery channel this preference controls.</summary>
    public NotificationChannel Channel { get; private set; }

    /// <summary>Whether the user wants this notification type on this channel.</summary>
    public bool IsEnabled { get; private set; }

    private NotificationPreference()
    {
    }

    private NotificationPreference(Guid userId, NotificationType type, NotificationChannel channel, bool isEnabled)
    {
        Id = ComputeId(userId, type, channel);
        UserId = userId;
        Type = type;
        Channel = channel;
        IsEnabled = isEnabled;
    }

    /// <summary>
    /// Computes the identity of the preference for a user, notification type and channel. The same triple
    /// always gives the same id.
    /// </summary>
    public static Guid ComputeId(Guid userId, NotificationType type, NotificationChannel channel)
    {
        var key = $"{userId:N}|{(int)type}|{(int)channel}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        return new Guid(hash.AsSpan(0, 16));
    }

    /// <summary>Creates a preference after validating it; it starts enabled unless stated otherwise.</summary>
    /// <returns>The new preference, or the rule that was broken.</returns>
    public static Result<NotificationPreference> Create(
        Guid userId, NotificationType type, NotificationChannel channel, bool isEnabled = true)
    {
        if (userId == Guid.Empty)
        {
            return Result.Failure<NotificationPreference>(NotificationErrors.UserRequired);
        }

        if (!Enum.IsDefined(type))
        {
            return Result.Failure<NotificationPreference>(NotificationErrors.InvalidType);
        }

        if (!Enum.IsDefined(channel))
        {
            return Result.Failure<NotificationPreference>(NotificationErrors.InvalidChannel);
        }

        return new NotificationPreference(userId, type, channel, isEnabled);
    }

    /// <summary>Turns this notification type on for the channel. Safe to call more than once.</summary>
    public void Enable() => IsEnabled = true;

    /// <summary>Turns this notification type off for the channel. Safe to call more than once.</summary>
    public void Disable() => IsEnabled = false;
}