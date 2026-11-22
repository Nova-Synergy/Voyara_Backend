using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides;

/// <summary>Errors raised by the guide request, session, chat and review use cases.</summary>
public static class GuideRequestErrors
{
    public static readonly Error RequestNotFound =
        Error.NotFound("GuideRequests.NotFound", "The guide request was not found.");

    public static readonly Error SessionNotFound =
        Error.NotFound("GuideSessions.NotFound", "The guide session was not found.");

    public static readonly Error NotRequestOwner =
        Error.Unauthorized("GuideRequests.Forbidden", "The request does not belong to the current user.");

    public static readonly Error NotAssignedGuide =
        Error.Unauthorized("GuideRequests.Forbidden", "The request is not addressed to the current guide.");

    public static readonly Error GuideProfileRequired =
        Error.Unauthorized("GuideRequests.GuideProfileRequired", "A guide profile is required for this operation.");

    public static readonly Error GuideNotEligible =
        Error.Conflict("GuideRequests.GuideNotEligible", "The guide is not currently accepting requests.");

    public static readonly Error OwnProfile =
        Error.Conflict("GuideRequests.OwnProfile", "You cannot send a request to your own guide profile.");

    public static readonly Error DateInPast =
        Error.Validation("GuideRequests.DateInPast", "The requested date cannot be in the past.");

    public static readonly Error GuideUnavailable =
        Error.Conflict("GuideRequests.GuideUnavailable", "The guide is not available at the requested date and time.");

    public static readonly Error DuplicateRequest =
        Error.Conflict("GuideRequests.Duplicate", "You already have an open request for this guide at the same date and time.");

    public static readonly Error SlotConflict =
        Error.Conflict("GuideRequests.SlotConflict", "The guide already has an accepted session that overlaps this request.");

    public static readonly Error ChatThreadNotFound =
        Error.NotFound("Chat.ThreadNotFound", "The chat thread was not found.");

    public static readonly Error NotChatParticipant =
        Error.Unauthorized("Chat.Forbidden", "You are not a participant of this chat.");

    public static readonly Error ChatThreadClosed =
        Error.Conflict("Chat.ThreadClosed", "The chat thread is closed.");

    public static readonly Error MessageNotFound =
        Error.NotFound("Chat.MessageNotFound", "The message was not found in this thread.");

    public static readonly Error ReviewNotFound =
        Error.NotFound("GuideReviews.NotFound", "The review was not found.");

    public static readonly Error ReviewNotAllowedYet =
        Error.Conflict("GuideReviews.SessionNotCompleted", "Only completed sessions can be reviewed.");

    public static readonly Error ReviewAlreadyExists =
        Error.Conflict("GuideReviews.Duplicate", "This session has already been reviewed.");
}
