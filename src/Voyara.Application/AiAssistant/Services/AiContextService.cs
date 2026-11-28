using System.Text;
using Voyara.Application.DTOs;
using Voyara.Domain.Entities.AiAssistant;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Catalog;

namespace Voyara.Application.AiAssistant.Services;

/// <summary>
/// Default implementation of <see cref="IAiContextService"/> integrating Catalog destinations
/// and <see cref="Conversation"/> state into AI prompts and context snapshots.
/// </summary>
public sealed class AiContextService : IAiContextService
{
    private readonly IDestinationRepository _destinationRepository;

    /// <summary>
    /// Initializes a new instance of <see cref="AiContextService"/>.
    /// </summary>
    public AiContextService(IDestinationRepository destinationRepository)
    {
        _destinationRepository = destinationRepository;
    }

    /// <inheritdoc />
    public async Task<AiConversationContextDto> BuildContextAsync(
        Conversation conversation,
        string? searchHint = null,
        int maxDestinations = 5,
        CancellationToken cancellationToken = default)
    {
        var take = Math.Clamp(maxDestinations, 1, 20);
        var effectiveSearch = !string.IsNullOrWhiteSpace(searchHint)
            ? searchHint.Trim()
            : conversation.Requirements?.Country;

        var candidates = await _destinationRepository.SearchAsync(
            searchTerm: string.IsNullOrWhiteSpace(effectiveSearch) ? null : effectiveSearch,
            status: DestinationStatus.Published,
            skip: 0,
            take: take,
            cancellationToken: cancellationToken);

        // Fall back to top published destinations if a specific search term yielded no matches
        if (candidates.Count == 0 && !string.IsNullOrWhiteSpace(effectiveSearch))
        {
            candidates = await _destinationRepository.SearchAsync(
                searchTerm: null,
                status: DestinationStatus.Published,
                skip: 0,
                take: take,
                cancellationToken: cancellationToken);
        }

        var catalogDestinations = candidates
            .Where(d => d.Status == DestinationStatus.Published)
            .OrderByDescending(d => d.PopularityScore)
            .ThenByDescending(d => d.AverageRating)
            .Take(take)
            .Select(d =>
            {
                var translation = d.GetTranslation(conversation.Language);
                return new CatalogContextDestinationDto(
                    d.Id,
                    translation.Name,
                    translation.Description,
                    d.AverageRating,
                    d.ReviewCount,
                    d.EstimatedVisitMinutes);
            })
            .ToList();

        TripRequirementsDto? requirementsDto = conversation.Requirements is null
            ? null
            : new TripRequirementsDto(
                conversation.Requirements.Country,
                conversation.Requirements.DurationDays,
                conversation.Requirements.Interests,
                conversation.Requirements.BudgetAmount,
                conversation.Requirements.BudgetCurrency,
                conversation.Requirements.TravelStyle,
                conversation.Requirements.TransportPreference,
                conversation.Requirements.IsComplete());

        return new AiConversationContextDto(
            conversation.Id,
            conversation.Language,
            conversation.State.ToString(),
            requirementsDto,
            catalogDestinations);
    }

    /// <inheritdoc />
    public string FormatPrompt(
        Conversation conversation,
        AiConversationContextDto context,
        int recentMessageCount = 8)
    {
        var promptBuilder = new StringBuilder();
        var systemInstruction = conversation.Language.StartsWith("ar", StringComparison.OrdinalIgnoreCase)
            ? "أنت مساعد السفر الذكي لتطبيق فويارا (Voyara). ساعد المستخدم في تخطيط الرحلات واقتراح الوجهات السياحية من الكتالوج المعتمد والإجابة بدقة ولطف باللغة العربية."
            : "You are the Voyara AI Travel Assistant. Help the tourist plan their trip, suggest attractions and destinations from the verified catalog, and answer travel questions concisely and helpfully.";

        promptBuilder.AppendLine($"[System: {systemInstruction}]");

        if (context.Requirements is not null)
        {
            var req = context.Requirements;
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Captured Trip Requirements:");
            if (!string.IsNullOrWhiteSpace(req.Country))
                promptBuilder.AppendLine($"- Country: {req.Country}");
            if (req.DurationDays > 0)
                promptBuilder.AppendLine($"- Duration: {req.DurationDays} days");
            if (req.BudgetAmount > 0)
                promptBuilder.AppendLine($"- Budget: {req.BudgetAmount} {req.BudgetCurrency}");
            if (req.Interests.Count > 0)
                promptBuilder.AppendLine($"- Interests: {string.Join(", ", req.Interests)}");
            if (!string.IsNullOrWhiteSpace(req.TravelStyle))
                promptBuilder.AppendLine($"- Travel Style: {req.TravelStyle}");
            if (!string.IsNullOrWhiteSpace(req.TransportPreference))
                promptBuilder.AppendLine($"- Transport Preference: {req.TransportPreference}");
        }

        if (context.CatalogDestinations.Count > 0)
        {
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Relevant Catalog Destinations:");
            foreach (var dest in context.CatalogDestinations)
            {
                promptBuilder.AppendLine(
                    $"- {dest.Name} (Rating: {dest.Rating:F1}★, {dest.ReviewCount} reviews, ~{dest.EstimatedVisitMinutes} min): {dest.Description}");
            }
        }

        promptBuilder.AppendLine();
        var recentHistory = conversation.Messages.TakeLast(Math.Max(1, recentMessageCount));
        foreach (var msg in recentHistory)
        {
            promptBuilder.AppendLine($"{msg.Role}: {msg.Content}");
        }

        promptBuilder.Append("Assistant: ");
        return promptBuilder.ToString();
    }
}
