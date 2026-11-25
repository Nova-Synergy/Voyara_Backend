using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides;

/// <summary>Application-level errors of the guide module.</summary>
public static class GuideErrors
{
    public static readonly Error NotAuthenticated =
        Error.Unauthorized("Guides.NotAuthenticated", "Authentication is required.");

    public static readonly Error ProfileNotFound =
        Error.NotFound("Guides.ProfileNotFound", "The guide profile was not found.");

    public static readonly Error ProfileAlreadyExists =
        Error.Conflict("Guides.ProfileAlreadyExists", "A guide profile already exists for this user.");

    public static Error InvalidStatus(string action, string currentStatus) =>
        Error.Conflict("Guides.InvalidStatus", $"Cannot {action} a profile in status '{currentStatus}'.");

    public static Error Incomplete(string description) =>
        Error.Validation("Guides.IncompleteApplication", description);
}
