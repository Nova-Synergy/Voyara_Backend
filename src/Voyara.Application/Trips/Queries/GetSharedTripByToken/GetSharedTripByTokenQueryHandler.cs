using MediatR;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Trips.Queries.GetSharedTripByToken;

/// <summary>
/// Handles retrieving trip details via an active share link token without requiring authentication.
/// </summary>
public sealed class GetSharedTripByTokenQueryHandler
    : IRequestHandler<GetSharedTripByTokenQuery, Result<TripResponse>>
{
    private readonly ITripRepository _tripRepository;

    /// <summary>
    /// Initializes a new instance of <see cref="GetSharedTripByTokenQueryHandler"/>.
    /// </summary>
    public GetSharedTripByTokenQueryHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    /// <inheritdoc />
    public async Task<Result<TripResponse>> Handle(
        GetSharedTripByTokenQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return Result.Failure<TripResponse>(Error.Validation(
                "TripShare.TokenRequired",
                "Share token is required."));
        }

        var normalizedToken = request.Token.Trim().ToLowerInvariant();
        var trip = await _tripRepository.GetByShareTokenAsync(normalizedToken, cancellationToken);

        if (trip is null)
        {
            return Result.Failure<TripResponse>(Error.NotFound(
                "TripShare.NotFound",
                "The shared trip link is invalid, expired, or has been revoked."));
        }

        var shares = trip.Shares
            .Select(s => new TripShareResponse(s.Id, s.Token, s.ExpiresAt, s.RevokedAt, s.IsActive))
            .ToList();

        return new TripResponse(
            trip.Id,
            trip.UserId,
            trip.Title,
            trip.StartDate,
            trip.EndDate,
            trip.Status.ToString(),
            trip.Visibility.ToString(),
            trip.JourneyId,
            shares);
    }
}
