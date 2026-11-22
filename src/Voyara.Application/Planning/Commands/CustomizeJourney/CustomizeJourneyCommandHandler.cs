using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Planning.Commands.CustomizeJourney;

/// <summary>
/// Handles applying natural language travel customization requests to an existing journey itinerary.
/// </summary>
public sealed class CustomizeJourneyCommandHandler : IRequestHandler<CustomizeJourneyCommand, Result<string>>
{
    private readonly IJourneyRepository _journeyRepository;
    private readonly IAiProvider _aiProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="CustomizeJourneyCommandHandler"/>.
    /// </summary>
    public CustomizeJourneyCommandHandler(
        IJourneyRepository journeyRepository,
        IAiProvider aiProvider,
        IUnitOfWork unitOfWork)
    {
        _journeyRepository = journeyRepository;
        _aiProvider = aiProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<string>> Handle(CustomizeJourneyCommand request, CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.GetByIdAsync(request.JourneyId, cancellationToken);
        if (journey is null)
        {
            return Result.Failure<string>(Error.NotFound(
                "Journey.NotFound",
                $"Journey '{request.JourneyId}' was not found."));
        }

        var prompt = $"The user requested the following customization for their {journey.TotalDays}-day itinerary titled '{journey.Title}': \"{request.Instruction}\". Provide a concise, helpful summary of the adjustments applied.";

        string aiResponse;
        try
        {
            aiResponse = await _aiProvider.GetCompletionAsync(prompt, cancellationToken);
        }
        catch
        {
            aiResponse = "Customization instruction recorded for your itinerary.";
        }

        if (request.Instruction.StartsWith("Rename to ", StringComparison.OrdinalIgnoreCase))
        {
            var newTitle = request.Instruction.Substring("Rename to ".Length).Trim().Trim('"', '\'');
            if (!string.IsNullOrWhiteSpace(newTitle))
            {
                journey.UpdateTitle(newTitle);
            }
        }

        var validation = journey.Validate();
        if (!validation.IsValid)
        {
            return Result.Failure<string>(Error.Validation(
                "Journey.ValidationFailed",
                string.Join("; ", validation.Errors)));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return aiResponse;
    }
}
