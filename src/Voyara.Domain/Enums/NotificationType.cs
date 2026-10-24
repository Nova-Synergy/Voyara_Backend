namespace Voyara.Domain.Enums;

/// <summary>
/// The kind of event a notification is about. The set follows the events described in the design document:
/// review moderation (SEQ-03), place suggestions (ST-02), report resolution (SEQ-12), guide requests
/// (SEQ-09, ACT-03), guide onboarding (SEQ-13) and chat.
/// </summary>
public enum NotificationType
{
    /// <summary>The user's review was approved.</summary>
    ReviewApproved = 1,

    /// <summary>The user's review was rejected.</summary>
    ReviewRejected = 2,

    /// <summary>The user's place suggestion was approved.</summary>
    PlaceSuggestionApproved = 3,

    /// <summary>The user's place suggestion was rejected.</summary>
    PlaceSuggestionRejected = 4,

    /// <summary>A report the user filed was resolved.</summary>
    ReportResolved = 5,

    /// <summary>A guide received a new request from a tourist.</summary>
    GuideRequestReceived = 6,

    /// <summary>A guide accepted the tourist's request.</summary>
    GuideRequestAccepted = 7,

    /// <summary>A guide rejected the tourist's request.</summary>
    GuideRequestRejected = 8,

    /// <summary>A request expired without an answer.</summary>
    GuideRequestExpired = 9,

    /// <summary>A guide profile was approved by an admin.</summary>
    GuideApproved = 10,

    /// <summary>A guide profile was rejected by an admin.</summary>
    GuideRejected = 11,

    /// <summary>A new chat message arrived.</summary>
    NewChatMessage = 12
}