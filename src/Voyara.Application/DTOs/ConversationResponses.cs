namespace Voyara.Application.DTOs;

/// <summary>A conversation as shown in the user's conversation list.</summary>
public sealed record ConversationSummaryResponse(
    Guid Id,
    string Language,
    string State,
    DateTime StartedAt,
    Guid? JourneyId,
    bool RequirementsComplete,
    int MessageCount);

/// <summary>One visible message of a conversation (system prompts are never exposed).</summary>
public sealed record MessageResponse(long Id, string Role, string Content, DateTime CreatedAt);

/// <summary>The trip requirements collected so far during a conversation.</summary>
public sealed record TripRequirementsResponse(
    string Country,
    int DurationDays,
    IReadOnlyList<string> Interests,
    decimal BudgetAmount,
    string BudgetCurrency,
    string TravelStyle,
    string TransportPreference,
    IReadOnlyList<string> MissingFields);

/// <summary>A conversation with its collected requirements and message history.</summary>
public sealed record ConversationDetailsResponse(
    Guid Id,
    string Language,
    string State,
    DateTime StartedAt,
    Guid? JourneyId,
    TripRequirementsResponse? Requirements,
    IReadOnlyList<MessageResponse> Messages);
