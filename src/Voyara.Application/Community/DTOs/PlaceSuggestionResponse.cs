using Voyara.Domain.Entities.Community;
using Voyara.Domain.Enums;

namespace Voyara.Application.Community.DTOs;

/// <summary>
/// Represents a place suggestion returned to the client.
/// </summary>
/// <param name="Id">The unique identifier of the place suggestion.</param>
/// <param name="UserId">The identifier of the user who submitted the suggestion.</param>
/// <param name="Name">The proposed name of the place.</param>
/// <param name="Description">A detailed description of the suggested place.</param>
/// <param name="Latitude">The geographic latitude coordinate of the place.</param>
/// <param name="Longitude">The geographic longitude coordinate of the place.</param>
/// <param name="Reason">The user's rationale for recommending this place.</param>
/// <param name="Status">The current moderation status of the suggestion.</param>
/// <param name="DecisionReason">The administrator's explanation for approving or rejecting the suggestion, if applicable.</param>
/// <param name="DestinationId">The identifier of the linked catalog destination once approved and published, if any.</param>
/// <param name="CreatedAt">The UTC timestamp when the suggestion was created.</param>
/// <param name="Photos">The collection of photos attached to the suggestion.</param>
public sealed record PlaceSuggestionResponse(
    Guid Id,
    Guid UserId,
    string Name,
    string Description,
    double Latitude,
    double Longitude,
    string Reason,
    SuggestionStatus Status,
    string? DecisionReason,
    Guid? DestinationId,
    DateTimeOffset? CreatedAt,
    IReadOnlyList<SuggestionPhotoResponse> Photos)
{
    /// <summary>
    /// Maps a <see cref="PlaceSuggestion"/> domain aggregate to a <see cref="PlaceSuggestionResponse"/> DTO.
    /// </summary>
    /// <param name="suggestion">The place suggestion aggregate to map.</param>
    /// <returns>The populated <see cref="PlaceSuggestionResponse"/> instance.</returns>
    public static PlaceSuggestionResponse From(PlaceSuggestion suggestion)
    {
        return new PlaceSuggestionResponse(
            suggestion.Id,
            suggestion.UserId,
            suggestion.Name,
            suggestion.Description,
            suggestion.Location.Latitude,
            suggestion.Location.Longitude,
            suggestion.Reason,
            suggestion.Status,
            suggestion.DecisionReason,
            suggestion.DestinationId,
            suggestion.CreatedAt,
            suggestion.Photos
                .Select(photo => new SuggestionPhotoResponse(
                    photo.Id,
                    photo.Url))
                .ToList());
    }
}

/// <summary>
/// Represents a photo attached to a place suggestion.
/// </summary>
/// <param name="Id">The unique identifier of the photo.</param>
/// <param name="Url">The accessible URL of the photo.</param>
public sealed record SuggestionPhotoResponse(Guid Id, string Url);