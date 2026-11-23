using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Trips.Queries.GetTripById;

/// <summary>
/// Handles retrieving a trip by its unique identifier with visibility access controls.
/// </summary>
public sealed class GetTripByIdQueryHandler : IRequestHandler<GetTripByIdQuery, Result<TripResponse>>
{
    private readonly ITripRepository _tripRepository;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of <see cref="GetTripByIdQueryHandler"/>.
    /// </summary>
    public GetTripByIdQueryHandler(ITripRepository tripRepository, ICurrentUser currentUser)
    {
        _tripRepository = tripRepository;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<TripResponse>> Handle(GetTripByIdQuery request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);
        if (trip is null)
        {
            return Result.Failure<TripResponse>(Error.NotFound(
                "Trip.NotFound",
                $"Trip '{request.TripId}' was not found."));
        }

        // Enforce privacy access control: public trips are readable by all; private trips only by owner/admin
        if (trip.Visibility == Visibility.Private &&
            _currentUser.UserId.HasValue &&
            _currentUser.UserId.Value != trip.UserId &&
            !_currentUser.IsInRole("Admin"))
        {
            return Result.Failure<TripResponse>(Error.Unauthorized(
                "Trip.Forbidden",
                "You do not have permission to view this trip."));
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
