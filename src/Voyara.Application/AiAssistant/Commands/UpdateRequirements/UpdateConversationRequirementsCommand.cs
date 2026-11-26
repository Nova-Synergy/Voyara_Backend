using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.AiAssistant.Commands.UpdateRequirements;

/// <summary>
/// Command to update or apply extracted travel requirements to an ongoing AI conversation.
/// </summary>
public sealed record UpdateConversationRequirementsCommand(
    Guid ConversationId,
    string Country,
    int DurationDays,
    List<string>? Interests,
    decimal BudgetAmount,
    string BudgetCurrency,
    string? TravelStyle,
    string? TransportPreference) : ICommand<TripRequirementsDto>;
