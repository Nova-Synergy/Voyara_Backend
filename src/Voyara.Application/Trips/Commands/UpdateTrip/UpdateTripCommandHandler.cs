using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Trips.Commands.UpdateTrip;

/// <summary>
/// Handles updating an existing trip aggregate, ensuring domain invariants and ownership checks.
/// </summary>
public sealed class UpdateTripCommandHandler : IRequestHandler<UpdateTripCommand, Result>
{
    private readonly ITripRepository _tripRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="UpdateTripCommandHandler"/>.
    /// </summary>
    public UpdateTripCommandHandler(
        ITripRepository tripRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(UpdateTripCommand request, CancellationToken cancellationToken)
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
                "You do not have permission to update this trip."));
        }

        trip.UpdateTitle(request.Title);
        trip.Reschedule(request.StartDate, request.EndDate);
        trip.UpdateVisibility(request.Visibility);

        if (request.Status.HasValue)
        {
            trip.ChangeStatus(request.Status.Value);
        }

        _tripRepository.Update(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
