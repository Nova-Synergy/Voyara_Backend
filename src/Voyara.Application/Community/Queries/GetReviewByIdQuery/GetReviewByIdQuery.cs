using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.Community.DTOs;

namespace Voyara.Application.Community.Queries.GetReviewById;

/// <summary>
/// Query to retrieve a review by its unique identifier.
/// </summary>
/// <param name="ReviewId">The unique identifier of the review to retrieve.</param>
/// <remarks>
/// Returns a ReviewResponse DTO containing the review data and its associated photos.
/// </remarks>
public sealed record GetReviewByIdQuery(Guid ReviewId)
    : IQuery<ReviewResponse>;