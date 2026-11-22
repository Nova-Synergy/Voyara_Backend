using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Catalog;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Planning.Commands.AddJourneyItem;

/// <summary>
/// Handles adding a destination stop to a journey day.
/// </summary>
public sealed class AddJourneyItemCommandHandler : IRequestHandler<AddJourneyItemCommand, Result<Guid>>
{
    private readonly IJourneyRepository _journeyRepository;
    private readonly IDestinationRepository _destinationRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="AddJourneyItemCommandHandler"/>.
    /// </summary>
    public AddJourneyItemCommandHandler(
        IJourneyRepository journeyRepository,
        IDestinationRepository destinationRepository,
        IUnitOfWork unitOfWork)
    {
        _journeyRepository = journeyRepository;
        _destinationRepository = destinationRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> Handle(AddJourneyItemCommand request, CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.GetByIdAsync(request.JourneyId, cancellationToken);
        if (journey is null)
        {
            return Result.Failure<Guid>(Error.NotFound(
                "Journey.NotFound",
                $"Journey '{request.JourneyId}' was not found."));
        }

        var destination = await _destinationRepository.GetByIdAsync(request.DestinationId, cancellationToken);
        if (destination is null)
        {
            return Result.Failure<Guid>(Error.NotFound(
                "Destination.NotFound",
                $"Destination '{request.DestinationId}' was not found."));
        }

        var transport = Enum.TryParse<TransportMode>(request.Transport, ignoreCase: true, out var parsedMode)
            ? parsedMode
            : TransportMode.Walking;

        var cityId = request.CityId ?? (destination.CityId != Guid.Empty ? destination.CityId : null);

        var item = journey.AddItem(
            request.DayNumber,
            request.DestinationId,
            cityId,
            request.StartTime,
            request.EndTime,
            transport);

        var validation = journey.Validate();
        if (!validation.IsValid)
        {
            return Result.Failure<Guid>(Error.Validation(
                "Journey.ValidationFailed",
                string.Join("; ", validation.Errors)));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return item.Id;
    }
}
