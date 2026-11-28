using Voyara.Application.DTOs;
using Voyara.Domain.Entities.AiAssistant;
using Voyara.Domain.Enums;

namespace Voyara.Application.AiAssistant;

/// <summary>Maps conversations to their read models.</summary>
internal static class ConversationMapping
{
    /// <summary>Creates the list item for a conversation; the message count comes from a separate query.</summary>
    public static ConversationSummaryResponse ToSummary(this Conversation conversation, int messageCount)
        => new(
            conversation.Id,
            conversation.Language,
            conversation.State.ToString(),
            conversation.StartedAt,
            conversation.JourneyId,
            conversation.Requirements?.IsComplete() ?? false,
            messageCount);

    /// <summary>Creates the detail view for a conversation loaded with its messages (System messages are hidden).</summary>
    public static ConversationDetailsResponse ToDetails(this Conversation conversation)
        => new(
            conversation.Id,
            conversation.Language,
            conversation.State.ToString(),
            conversation.StartedAt,
            conversation.JourneyId,
            conversation.Requirements is null ? null : ToResponse(conversation.Requirements),
            conversation.Messages
                .Where(message => message.Role != MessageRole.System)
                .OrderBy(message => message.Id)
                .Select(message => new MessageResponse(
                    message.Id,
                    message.Role.ToString(),
                    message.Content,
                    message.CreatedAt))
                .ToList());

    private static TripRequirementsResponse ToResponse(Voyara.Domain.ValueObjects.TripRequirements requirements)
        => new(
            requirements.Country,
            requirements.DurationDays,
            requirements.Interests,
            requirements.BudgetAmount,
            requirements.BudgetCurrency,
            requirements.TravelStyle,
            requirements.TransportPreference,
            requirements.MissingFields());
}
