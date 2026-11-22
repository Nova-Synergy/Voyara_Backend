using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Planning.Commands.RemoveJourneyItem;

/// <summary>
/// Handles removing a destination stop from a journey itinerary.
/// </summary>
public sealed class RemoveJourneyItemCommandHandler : IRequestHandler<RemoveJourneyItemCommand, Result>
{
    private readonly IJourneyRepository _journeyRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="RemoveJourneyItemCommandHandler"/>.
    /// </summary>
    public RemoveJourneyItemCommandHandler(
        IJourneyRepository journeyRepository,
        IUnitOfWork unitOfWork)
    {
        _journeyRepository = journeyRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(RemoveJourneyItemCommand request, CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.GetByIdAsync(request.JourneyId, cancellationToken);
        if (journey is null)
        {
            return Result.Failure(Error.NotFound(
                "Journey.NotFound",
                $"Journey '{request.JourneyId}' was not found."));
        }

        var removed = journey.RemoveItem(request.ItemId);
        if (!removed)
        {
            return Result.Failure(Error.NotFound(
                "Journey.ItemNotFound",
                $"Item '{request.ItemId}' was not found in journey '{request.JourneyId}'."));
        }

        var validation = journey.Validate();
        if (!validation.IsValid)
        {
            return Result.Failure(Error.Validation(
                "Journey.ValidationFailed",
                string.Join("; ", validation.Errors)));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
