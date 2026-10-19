using Voyara.Domain.Enums;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Community;

/// <summary>Expected business errors produced by the PlaceSuggestion aggregate.</summary>
public static class PlaceSuggestionErrors
{
    public static readonly Error UserRequired =
        Error.Validation("PlaceSuggestion.UserRequired", "The submitting user is required.");

    public static readonly Error NameRequired =
        Error.Validation("PlaceSuggestion.NameRequired", "Place name is required.");

    public static readonly Error DescriptionRequired =
        Error.Validation("PlaceSuggestion.DescriptionRequired", "Description is required.");

    public static readonly Error LocationRequired =
        Error.Validation("PlaceSuggestion.LocationRequired", "Location is required.");

    public static readonly Error ReasonRequired =
        Error.Validation("PlaceSuggestion.ReasonRequired", "A reason for the suggestion is required.");

    public static readonly Error InvalidPhotoUrl =
        Error.Validation("PlaceSuggestion.InvalidPhotoUrl", "Photo URL cannot be empty.");

    public static readonly Error PhotoNotFound =
        Error.NotFound("PlaceSuggestion.PhotoNotFound", "The photo does not belong to this suggestion.");

    public static readonly Error AdminRequired =
        Error.Validation("PlaceSuggestion.AdminRequired", "An administrator ID is required.");

    public static readonly Error DecisionReasonRequired =
        Error.Validation("PlaceSuggestion.DecisionReasonRequired", "A decision reason is required.");

    public static readonly Error DestinationRequired =
        Error.Validation("PlaceSuggestion.DestinationRequired", "A valid destination ID is required.");

    public static readonly Error NotApproved =
        Error.Conflict("PlaceSuggestion.NotApproved", "Only an approved suggestion can be linked to a destination.");

    public static readonly Error AlreadyLinked =
        Error.Conflict("PlaceSuggestion.AlreadyLinked", "This suggestion is already linked to a destination.");

    public static Error InvalidTransition(string operation, SuggestionStatus current) =>
        Error.Conflict(
            "PlaceSuggestion.InvalidTransition",
            $"Cannot {operation} a suggestion that is {current}.");
}