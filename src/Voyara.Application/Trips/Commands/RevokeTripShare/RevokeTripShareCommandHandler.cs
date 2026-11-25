using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Trips.Commands.RevokeTripShare;

/// <summary>
/// Handles revoking an active trip share token, verifying existence and ownership permissions.
/// </summary>
public sealed class RevokeTripShareCommandHandler : IRequestHandler<RevokeTripShareCommand, Result>
{
    private readonly ITripRepository _tripRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="RevokeTripShareCommandHandler"/>.
    /// </summary>
    public RevokeTripShareCommandHandler(
        ITripRepository tripRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(RevokeTripShareCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);
        if (trip is null)
        {
            return Result.Failure(Error.NotFound(
                "Trip.NotFound",
                $"Trip '{request.TripId}' was not found."));
        }

        // Verify ownership or admin privilege
        if (_currentUser.UserId.HasValue &&
            _currentUser.UserId.Value != trip.UserId &&
            !_currentUser.IsInRole("Admin"))
        {
            return Result.Failure(Error.Unauthorized(
                "Trip.Forbidden",
                "You do not have permission to revoke shares for this trip."));
        }

        var share = trip.Shares.FirstOrDefault(s => s.Token == request.Token);
        if (share is null)
        {
            return Result.Failure(Error.NotFound(
                "TripShare.NotFound",
                $"Share token '{request.Token}' was not found for this trip."));
        }

        trip.RevokeShare(request.Token);
        _tripRepository.Update(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
