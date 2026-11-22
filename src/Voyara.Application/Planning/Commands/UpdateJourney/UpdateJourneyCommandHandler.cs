using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Planning.Commands.UpdateJourney;

/// <summary>
/// Handles updating journey title and lifecycle status.
/// </summary>
public sealed class UpdateJourneyCommandHandler : IRequestHandler<UpdateJourneyCommand, Result>
{
    private readonly IJourneyRepository _journeyRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="UpdateJourneyCommandHandler"/>.
    /// </summary>
    public UpdateJourneyCommandHandler(
        IJourneyRepository journeyRepository,
        IUnitOfWork unitOfWork)
    {
        _journeyRepository = journeyRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(UpdateJourneyCommand request, CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.GetByIdAsync(request.JourneyId, cancellationToken);
        if (journey is null)
        {
            return Result.Failure(Error.NotFound(
                "Journey.NotFound",
                $"Journey '{request.JourneyId}' was not found."));
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            journey.UpdateTitle(request.Title.Trim());
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<JourneyStatus>(request.Status, true, out var newStatus))
        {
            journey.ChangeStatus(newStatus);
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
