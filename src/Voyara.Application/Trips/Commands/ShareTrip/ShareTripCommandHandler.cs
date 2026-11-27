using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Trips.Commands.ShareTrip;

/// <summary>
/// Handles generating a new share token for a trip, elevating visibility to Shared and persisting changes.
/// </summary>
public sealed class ShareTripCommandHandler : IRequestHandler<ShareTripCommand, Result<TripShareResponse>>
{
    private readonly ITripRepository _tripRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="ShareTripCommandHandler"/>.
    /// </summary>
    public ShareTripCommandHandler(
        ITripRepository tripRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<TripShareResponse>> Handle(ShareTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);
        if (trip is null)
        {
            return Result.Failure<TripShareResponse>(Error.NotFound(
                "Trip.NotFound",
                $"Trip '{request.TripId}' was not found."));
        }

        // Verify ownership or admin privilege
        if (_currentUser.UserId.HasValue &&
            _currentUser.UserId.Value != trip.UserId &&
            !_currentUser.IsInRole("Admin"))
        {
            return Result.Failure<TripShareResponse>(Error.Unauthorized(
                "Trip.Forbidden",
                "You do not have permission to share this trip."));
        }

        var share = trip.Share(request.ExpiresAt);
        _tripRepository.Update(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TripShareResponse(
            share.Id,
            share.Token,
            share.ExpiresAt,
            share.RevokedAt,
            share.IsActive);
    }
}
