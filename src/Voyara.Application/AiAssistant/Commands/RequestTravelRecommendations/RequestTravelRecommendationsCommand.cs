using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.AiAssistant.Commands.RequestTravelRecommendations;

/// <summary>
/// Command to request personalized travel recommendations within an AI assistant conversation session.
/// </summary>
/// <param name="ConversationId">The conversation session identifier.</param>
/// <param name="Prompt">Optional tourist inquiry or theme.</param>
/// <param name="CityId">Optional city filter.</param>
/// <param name="CategoryId">Optional category filter.</param>
/// <param name="Limit">Maximum recommendations to return (defaults to 3).</param>
public sealed record RequestTravelRecommendationsCommand(
    Guid ConversationId,
    string? Prompt = null,
    Guid? CityId = null,
    Guid? CategoryId = null,
    int Limit = 3) : ICommand<ConversationalRecommendationResponse>;
