namespace Voyara.Api.Contracts.Planning;

/// <summary>
/// HTTP request contract for adding a destination stop to a journey day.
/// </summary>
/// <param name="DayNumber">1-based day number.</param>
/// <param name="DestinationId">Catalog destination unique identifier.</param>
/// <param name="CityId">Optional city unique identifier.</param>
/// <param name="StartTime">Optional scheduled start time.</param>
/// <param name="EndTime">Optional scheduled end time.</param>
/// <param name="Transport">Optional transportation mode name (defaults to "Walking").</param>
public sealed record AddJourneyItemRequest(
    int DayNumber,
    Guid DestinationId,
    Guid? CityId,
    TimeSpan? StartTime,
    TimeSpan? EndTime,
    string? Transport);
