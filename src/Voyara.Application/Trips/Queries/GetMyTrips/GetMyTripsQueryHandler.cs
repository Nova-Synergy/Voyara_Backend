using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Trips.Queries.GetMyTrips;

/// <summary>
/// Handles listing the authenticated tourist's trips with pagination and optional status filtering.
/// </summary>
public sealed class GetMyTripsQueryHandler : IRequestHandler<GetMyTripsQuery, Result<PagedResult<TripResponse>>>
{
    private readonly ITripRepository _tripRepository;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of <see cref="GetMyTripsQueryHandler"/>.
    /// </summary>
    public GetMyTripsQueryHandler(ITripRepository tripRepository, ICurrentUser currentUser)
    {
        _tripRepository = tripRepository;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<TripResponse>>> Handle(GetMyTripsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (!userId.HasValue)
        {
            return Result.Failure<PagedResult<TripResponse>>(Error.Unauthorized(
                "Identity.Unauthenticated",
                "User must be authenticated to list their trips."));
        }

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        var (trips, totalCount) = await _tripRepository.GetPagedByUserIdAsync(
            userId.Value,
            request.Status,
            skip,
            pageSize,
            cancellationToken);

        var items = trips
            .Select(t => new TripResponse(
                t.Id,
                t.UserId,
                t.Title,
                t.StartDate,
                t.EndDate,
                t.Status.ToString(),
                t.Visibility.ToString(),
                t.JourneyId,
                t.Shares.Select(s => new TripShareResponse(s.Id, s.Token, s.ExpiresAt, s.RevokedAt, s.IsActive)).ToList()))
            .ToList();

        return new PagedResult<TripResponse>(items, totalCount, page, pageSize);
    }
}
