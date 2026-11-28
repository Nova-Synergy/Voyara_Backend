namespace Voyara.Api.Contracts.AiAssistant;

/// <summary>Optional request body of the start-conversation endpoint.</summary>
/// <param name="Language">Language of the conversation (for example en or ar); English when omitted.</param>
public sealed record StartConversationRequest(string? Language);
