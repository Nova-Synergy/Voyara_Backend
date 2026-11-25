using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Trips.Commands.DeleteTrip;

/// <summary>
/// Handles deleting a trip after verifying existence and ownership permissions.
/// </summary>
public sealed class DeleteTripCommandHandler : IRequestHandler<DeleteTripCommand, Result>
{
    private readonly ITripRepository _tripRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="DeleteTripCommandHandler"/>.
    /// </summary>
    public DeleteTripCommandHandler(
        ITripRepository tripRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(DeleteTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);
        if (trip is null)
        {
            return Result.Failure(Error.NotFound(
                "Trip.NotFound",
                $"Trip '{request.TripId}' was not found."));
        }

        // Check ownership or admin privilege
        if (_currentUser.UserId.HasValue &&
            _currentUser.UserId.Value != trip.UserId &&
            !_currentUser.IsInRole("Admin"))
        {
            return Result.Failure(Error.Unauthorized(
                "Trip.Forbidden",
                "You do not have permission to delete this trip."));
        }

        _tripRepository.Delete(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
