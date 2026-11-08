using MediatR;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Planning.Queries.GetJourneyById;

/// <summary>
/// Handles retrieving a single journey itinerary by identifier.
/// </summary>
public sealed class GetJourneyByIdQueryHandler
    : IRequestHandler<GetJourneyByIdQuery, Result<JourneyResponse>>
{
    private readonly IJourneyRepository _journeyRepository;

    /// <summary>
    /// Initializes a new instance of <see cref="GetJourneyByIdQueryHandler"/>.
    /// </summary>
    /// <param name="journeyRepository">The repository used to access journey aggregates.</param>
    public GetJourneyByIdQueryHandler(IJourneyRepository journeyRepository)
    {
        _journeyRepository = journeyRepository;
    }

    /// <inheritdoc />
    public async Task<Result<JourneyResponse>> Handle(
        GetJourneyByIdQuery request,
        CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.GetByIdAsync(request.JourneyId, cancellationToken);
        if (journey is null)
        {
            return Result.Failure<JourneyResponse>(Error.NotFound(
                "Journey.NotFound",
                $"Journey '{request.JourneyId}' was not found."));
        }

        var days = journey.Days
            .OrderBy(d => d.DayNumber)
            .Select(day => new JourneyDayResponse(
                day.Id,
                day.DayNumber,
                day.Date,
                day.Items
                    .OrderBy(i => i.SortOrder)
                    .Select(item => new JourneyItemResponse(
                        item.Id,
                        item.DestinationId,
                        item.CityId,
                        item.SortOrder,
                        item.StartTime,
                        item.EndTime,
                        item.Transport.ToString(),
                        item.Visited))
                    .ToList()))
            .ToList();

        var response = new JourneyResponse(
            journey.Id,
            journey.TripId,
            journey.Title,
            journey.TotalDays,
            journey.GeneratedByAI,
            journey.Status.ToString(),
            days);

        return response;
    }
}
