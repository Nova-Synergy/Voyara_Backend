using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.Community.DTOs;

namespace Voyara.Application.Community.Queries.GetMyReviews;

/// <summary>
/// Query to retrieve a list of reviews submitted by the currently authenticated user.</summary>
/// <remarks>
/// <para>
/// Requires authorization; the current user must be authenticated to execute.</para>
/// <para>
/// Returns a list of ReviewResponse DTOs containing the user's review data and associated photos.</para>
/// </remarks>
[AuthorizeRequest(Roles.Tourist)]
public sealed record GetMyReviewsQuery(int Page = 1, int PageSize = 10)
    : IQuery<IReadOnlyList<ReviewResponse>>;