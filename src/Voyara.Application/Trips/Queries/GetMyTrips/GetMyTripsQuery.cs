using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Domain.Enums;

namespace Voyara.Application.Trips.Queries.GetMyTrips;

/// <summary>
/// Query to retrieve a paginated listing of trips belonging to the authenticated user.
/// </summary>
/// <param name="Page">1-based page number.</param>
/// <param name="PageSize">Number of items to return per page.</param>
/// <param name="Status">Optional status filter.</param>
public sealed record GetMyTripsQuery(
    int Page = 1,
    int PageSize = 10,
    TripStatus? Status = null) : IQuery<PagedResult<TripResponse>>;
