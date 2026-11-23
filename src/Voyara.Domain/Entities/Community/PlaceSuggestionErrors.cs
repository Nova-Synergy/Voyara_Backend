using Voyara.Domain.Enums;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Community;

/// <summary>Expected business errors produced by the PlaceSuggestion aggregate.</summary>
public static class PlaceSuggestionErrors
{
    /// <summary>Error returned when the submitting user identifier is missing or empty.</summary>
    public static readonly Error UserRequired =
        Error.Validation("PlaceSuggestion.UserRequired", "The submitting user is required.");

    /// <summary>Error returned when the suggested place name is missing or empty.</summary>
    public static readonly Error NameRequired =
        Error.Validation("PlaceSuggestion.NameRequired", "Place name is required.");

    /// <summary>Error returned when the suggestion description is missing or empty.</summary>
    public static readonly Error DescriptionRequired =
        Error.Validation("PlaceSuggestion.DescriptionRequired", "Description is required.");

    /// <summary>Error returned when the suggestion location is missing or empty.</summary>
    public static readonly Error LocationRequired =
        Error.Validation("PlaceSuggestion.LocationRequired", "Location is required.");

    /// <summary>Error returned when a reason for the suggestion is missing or empty.</summary>
    public static readonly Error ReasonRequired =
        Error.Validation("PlaceSuggestion.ReasonRequired", "A reason for the suggestion is required.");

    /// <summary>Error returned when a photo URL is null, whitespace, or otherwise invalid.</summary>
    public static readonly Error InvalidPhotoUrl =
        Error.Validation("PlaceSuggestion.InvalidPhotoUrl", "Photo URL cannot be empty.");

    /// <summary>Error returned when the specified photo is not associated with the suggestion.</summary>
    public static readonly Error PhotoNotFound =
        Error.NotFound("PlaceSuggestion.PhotoNotFound", "The photo does not belong to this suggestion.");

    /// <summary>Error returned when an administrator identifier is missing or empty during moderation.</summary>
    public static readonly Error AdminRequired =
        Error.Validation("PlaceSuggestion.AdminRequired", "An administrator ID is required.");

    /// <summary>Error returned when a moderation decision reason is missing or empty.</summary>
    public static readonly Error DecisionReasonRequired =
        Error.Validation("PlaceSuggestion.DecisionReasonRequired", "A decision reason is required.");

    /// <summary>Error returned when a valid destination identifier is not provided for linking.</summary>
    public static readonly Error DestinationRequired =
        Error.Validation("PlaceSuggestion.DestinationRequired", "A valid destination ID is required.");

    /// <summary>Error returned when attempting to link a suggestion that has not been approved.</summary>
    public static readonly Error NotApproved =
        Error.Conflict("PlaceSuggestion.NotApproved", "Only an approved suggestion can be linked to a destination.");

    /// <summary>Error returned when attempting to link a suggestion that is already linked to a destination.</summary>
    public static readonly Error AlreadyLinked =
        Error.Conflict("PlaceSuggestion.AlreadyLinked", "This suggestion is already linked to a destination.");

    /// <summary>Error returned when a user attempts to modify a suggestion they do not own.</summary>
    public static readonly Error NotOwner =
        Error.Validation(
            "PlaceSuggestion.NotOwner",
            "You can only modify your own place suggestions.");

    /// <summary>Error returned when a user attempts to modify an already deleted suggestion.</summary>
    public static readonly Error AlreadyDeleted =
        Error.Conflict(
            "PlaceSuggestion.AlreadyDeleted",
            "This place suggestion has already been deleted.");

    /// <summary>
    /// Creates a conflict error when an operation is not permitted for the suggestion's current status.
    /// </summary>
    /// <param name="operation">The name or description of the attempted operation.</param>
    /// <param name="current">The current <see cref="SuggestionStatus"/> of the suggestion.</param>
    /// <returns>An <see cref="Error"/> describing the invalid state transition.</returns>
    public static Error InvalidTransition(string operation, SuggestionStatus current) =>
        Error.Conflict(
            "PlaceSuggestion.InvalidTransition",
            $"Cannot {operation} a suggestion that is {current}.");
}