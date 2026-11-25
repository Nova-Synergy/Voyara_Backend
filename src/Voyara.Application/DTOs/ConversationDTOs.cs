namespace Voyara.Application.DTOs;

/// <summary>
/// Payload to create a new conversational AI session.
/// </summary>
/// <param name="Language">Optional ISO language code (e.g. "en", "ar").</param>
public sealed record CreateConversationRequest(string? Language);

/// <summary>
/// Payload to send a user message to the AI assistant.
/// </summary>
/// <param name="Content">The user's conversational prompt.</param>
public sealed record SendMessageRequest(string Content);

/// <summary>
/// Read model representing a single conversational turn.
/// </summary>
/// <param name="Id">Unique message identifier.</param>
/// <param name="Role">Author role (User, Assistant, System).</param>
/// <param name="Content">Textual content payload.</param>
/// <param name="TokensUsed">Tokens consumed by this turn.</param>
/// <param name="CreatedAt">Message timestamp in UTC.</param>
public sealed record MessageResponse(
    long Id,
    string Role,
    string Content,
    int TokensUsed,
    DateTime CreatedAt);

/// <summary>
/// Summary read model of a conversation session for listing views.
/// </summary>
/// <param name="Id">Conversation identifier.</param>
/// <param name="Language">Interaction language.</param>
/// <param name="State">Current conversation workflow state.</param>
/// <param name="StartedAt">Session start time in UTC.</param>
/// <param name="MessageCount">Total messages exchanged.</param>
/// <param name="JourneyId">Associated journey identifier, if generated.</param>
public sealed record ConversationSummaryResponse(
    Guid Id,
    string Language,
    string State,
    DateTime StartedAt,
    int MessageCount,
    Guid? JourneyId);

/// <summary>
/// Detailed read model representing an AI conversation session and its complete message history.
/// </summary>
/// <param name="Id">Conversation identifier.</param>
/// <param name="Language">Interaction language.</param>
/// <param name="State">Current conversation workflow state.</param>
/// <param name="StartedAt">Session start time in UTC.</param>
/// <param name="JourneyId">Associated journey identifier, if generated.</param>
/// <param name="Messages">Ordered message history.</param>
public sealed record ConversationDetailResponse(
    Guid Id,
    string Language,
    string State,
    DateTime StartedAt,
    Guid? JourneyId,
    IReadOnlyList<MessageResponse> Messages);
