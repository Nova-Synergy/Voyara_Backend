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

    public static readonly Error DocumentNotFound =
        Error.NotFound("Guides.DocumentNotFound", "The document was not found.");

    public static readonly Error TooManyDocuments =
        Error.Conflict("Guides.TooManyDocuments", "The maximum number of documents has been reached.");

    public static Error InvalidStatus(string action, string currentStatus) =>
        Error.Conflict("Guides.InvalidStatus", $"Cannot {action} a profile in status '{currentStatus}'.");

    public static Error InvalidDocument(string description) =>
        Error.Validation("Guides.InvalidDocument", description);

    public static Error Incomplete(string description) =>
        Error.Validation("Guides.IncompleteApplication", description);
}
