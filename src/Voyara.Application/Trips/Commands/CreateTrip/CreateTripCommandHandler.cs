using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.TripAndJourney;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Trips.Commands.CreateTrip;

/// <summary>
/// Handles creating a new trip aggregate, linking an optional journey, and persisting changes.
/// </summary>
public sealed class CreateTripCommandHandler : IRequestHandler<CreateTripCommand, Result<Guid>>
{
    private readonly ITripRepository _tripRepository;
    private readonly IJourneyRepository _journeyRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="CreateTripCommandHandler"/>.
    /// </summary>
    public CreateTripCommandHandler(
        ITripRepository tripRepository,
        IJourneyRepository journeyRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _journeyRepository = journeyRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> Handle(CreateTripCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? Guid.NewGuid();

        if (request.JourneyId.HasValue)
        {
            var journeyExists = await _journeyRepository.ExistsAsync(request.JourneyId.Value, cancellationToken);
            if (!journeyExists)
            {
                return Result.Failure<Guid>(Error.NotFound(
                    "Journey.NotFound",
                    $"Journey '{request.JourneyId.Value}' was not found."));
            }
        }

        var trip = Trip.Create(
            userId,
            request.Title,
            request.StartDate,
            request.EndDate,
            request.Visibility);

        if (request.JourneyId.HasValue)
        {
            trip.AttachJourney(request.JourneyId.Value);
        }

        _tripRepository.Add(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return trip.Id;
    }
}
