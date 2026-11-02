using Voyara.Domain.Entities.Community;
using Voyara.Domain.Enums;

namespace Voyara.Application.Community.DTOs;

/// <summary>
/// Review data returned by the API.
/// </summary>
/// <param name="Id">The unique identifier of the review.</param>
/// <param name="UserId">The identifier of the user who submitted the review.</param>
/// <param name="DestinationId">The identifier of the reviewed destination.</param>
/// <param name="Rating">The rating given, from one to five stars.</param>
/// <param name="Text">The written review content.</param>
/// <param name="VisitDate">The date the user visited the destination.</param>
/// <param name="Status">The current moderation status of the review.</param>
/// <param name="CreatedAt">The date and time when the review was created.</param>
/// <param name="Photos">The collection of photos attached to the review.</param>
public sealed record ReviewResponse(
    Guid Id,
    Guid UserId,
    Guid DestinationId,
    int Rating,
    string Text,
    DateOnly VisitDate,
    ReviewStatus Status,
    DateTimeOffset? CreatedAt,
    IReadOnlyList<ReviewPhotoResponse> Photos)
{
    /// <summary>
    /// Maps a Review aggregate to a ReviewResponse DTO.
    /// </summary>
    /// <param name="review">The review aggregate to map.</param>
    /// <returns>A ReviewResponse containing the review data and its associated photos.</returns>
    public static ReviewResponse From(Review review)
    {
        return new ReviewResponse(
            review.Id,
            review.UserId,
            review.DestinationId,
            review.Rating,
            review.Text,
            review.VisitDate,
            review.Status,
            review.CreatedAt,
            review.Photos
                .Select(photo => new ReviewPhotoResponse(photo.Id, photo.Url))
                .ToList());
    }
}

/// <summary>
/// Photo information returned with a review.
/// </summary>
/// <param name="Id">The unique identifier of the photo.</param>
/// <param name="Url">The URL location of the stored image.</param>
public sealed record ReviewPhotoResponse(Guid Id, string Url);