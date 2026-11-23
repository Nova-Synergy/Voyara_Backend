namespace Voyara.Application.DTOs;

/// <summary>
/// Read model representing a scheduled stop or activity within a journey day.
/// </summary>
public sealed record JourneyItemResponse(
    Guid Id,
    Guid DestinationId,
    Guid? CityId,
    int SortOrder,
    TimeSpan? StartTime,
    TimeSpan? EndTime,
    string Transport,
    bool Visited);

/// <summary>
/// Read model representing a single day in a journey itinerary.
/// </summary>
public sealed record JourneyDayResponse(
    Guid Id,
    int DayNumber,
    DateOnly? Date,
    IReadOnlyList<JourneyItemResponse> Items);

/// <summary>
/// Read model representing an end-to-end journey itinerary.
/// </summary>
public sealed record JourneyResponse(
    Guid Id,
    Guid? TripId,
    string Title,
    int TotalDays,
    bool GeneratedByAI,
    string Status,
    IReadOnlyList<JourneyDayResponse> Days);
