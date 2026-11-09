using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Entities.TripAndJourney;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Catalog;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Planning.Commands.GenerateJourney;

/// <summary>
/// Handles generating a new AI journey itinerary from requirements and destination catalog candidates.
/// </summary>
public sealed class GenerateJourneyCommandHandler : IRequestHandler<GenerateJourneyCommand, Result<Guid>>
{
    private readonly IJourneyRepository _journeyRepository;
    private readonly IDestinationRepository _destinationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiProvider _aiProvider;

    /// <summary>
    /// Initializes a new instance of <see cref="GenerateJourneyCommandHandler"/>.
    /// </summary>
    public GenerateJourneyCommandHandler(
        IJourneyRepository journeyRepository,
        IDestinationRepository destinationRepository,
        IUnitOfWork unitOfWork,
        IAiProvider aiProvider)
    {
        _journeyRepository = journeyRepository;
        _destinationRepository = destinationRepository;
        _unitOfWork = unitOfWork;
        _aiProvider = aiProvider;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> Handle(GenerateJourneyCommand request, CancellationToken cancellationToken)
    {
        var duration = Math.Clamp(request.DurationDays, 1, 30);

        // 1. Resolve or synthesize Journey Title
        string title = request.Title?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(title))
        {
            try
            {
                var prompt = $"Suggest a short, inspiring travel itinerary title for a {duration}-day trip to {request.Country ?? "selected destinations"} focusing on {string.Join(", ", request.Interests)}.";
                var aiTitle = await _aiProvider.GetCompletionAsync(prompt, cancellationToken);
                if (!string.IsNullOrWhiteSpace(aiTitle) && !aiTitle.Contains("[stub response]"))
                {
                    title = aiTitle.Trim().Trim('"');
                }
            }
            catch
            {
                // Fall back gracefully if AI service fails or is unreachable
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                var region = !string.IsNullOrWhiteSpace(request.Country) ? request.Country : "Featured Destinations";
                title = $"{region} Itinerary ({duration} Days)";
            }
        }

        // 2. Resolve preferred transport mode
        var transport = Enum.TryParse<TransportMode>(request.TransportPreference, ignoreCase: true, out var parsedMode)
            ? parsedMode
            : TransportMode.Walking;

        // 3. Query candidate destinations from Catalog
        var maxStopsNeeded = duration * 4;
        var candidates = await _destinationRepository.SearchAsync(
            searchTerm: request.Country,
            status: DestinationStatus.Published,
            skip: 0,
            take: maxStopsNeeded,
            cancellationToken: cancellationToken);

        if (candidates.Count == 0)
        {
            candidates = await _destinationRepository.SearchAsync(
                searchTerm: null,
                status: DestinationStatus.Published,
                skip: 0,
                take: maxStopsNeeded,
                cancellationToken: cancellationToken);
        }

        if (candidates.Count == 0)
        {
            candidates = await _destinationRepository.SearchAsync(
                searchTerm: null,
                status: null,
                skip: 0,
                take: maxStopsNeeded,
                cancellationToken: cancellationToken);
        }

        // 4. Create the Journey aggregate root (generatedByAI: true)
        var journey = Journey.Create(title, duration, generatedByAI: true);

        // 5. Determine stops per day schedule based on TravelStyle
        var stopsPerDay = ResolveStopsPerDay(request.TravelStyle);
        var timeWindows = GetTimeWindows(stopsPerDay);

        int candidateIndex = 0;
        for (int dayNumber = 1; dayNumber <= duration; dayNumber++)
        {
            for (int slot = 0; slot < stopsPerDay && candidateIndex < candidates.Count; slot++)
            {
                var destination = candidates[candidateIndex++];
                var window = timeWindows[slot];

                journey.AddItem(
                    dayNumber,
                    destination.Id,
                    destination.CityId != Guid.Empty ? destination.CityId : null,
                    window.StartTime,
                    window.EndTime,
                    transport);
            }
        }

        // 6. Validate domain invariants
        var validation = journey.Validate();
        if (!validation.IsValid)
        {
            return Result.Failure<Guid>(Error.Validation(
                "Journey.ValidationFailed",
                string.Join("; ", validation.Errors)));
        }

        // 7. Persist via repository and Unit of Work
        _journeyRepository.Add(journey);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return journey.Id;
    }

    private static int ResolveStopsPerDay(string? travelStyle)
    {
        if (string.IsNullOrWhiteSpace(travelStyle))
        {
            return 2;
        }

        var normalized = travelStyle.Trim().ToLowerInvariant();
        if (normalized.Contains("relax") || normalized.Contains("slow"))
        {
            return 2;
        }

        if (normalized.Contains("adventure") || normalized.Contains("active") || normalized.Contains("fast"))
        {
            return 3;
        }

        return 2;
    }

    private static IReadOnlyList<(TimeSpan StartTime, TimeSpan EndTime)> GetTimeWindows(int count)
    {
        var windows = new List<(TimeSpan, TimeSpan)>
        {
            (new TimeSpan(9, 30, 0), new TimeSpan(12, 30, 0)),
            (new TimeSpan(14, 0, 0), new TimeSpan(16, 30, 0)),
            (new TimeSpan(17, 30, 0), new TimeSpan(20, 0, 0)),
            (new TimeSpan(20, 30, 0), new TimeSpan(22, 0, 0))
        };

        return windows.Take(count).ToList();
    }
}
