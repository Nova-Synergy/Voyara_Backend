using System.Text;
using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant.Commands.RequestTravelRecommendations;

/// <summary>
/// Handles retrieving candidate recommendations, prompting the local Ollama LLM for a personalized pitch, and recording the interaction.
/// </summary>
public sealed class RequestTravelRecommendationsCommandHandler
    : IRequestHandler<RequestTravelRecommendationsCommand, Result<ConversationalRecommendationResponse>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IDestinationRepository _destinationRepository;
    private readonly IRecommendationStrategy _recommendationStrategy;
    private readonly IAiProvider _aiProvider;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="RequestTravelRecommendationsCommandHandler"/>.
    /// </summary>
    public RequestTravelRecommendationsCommandHandler(
        IConversationRepository conversationRepository,
        IDestinationRepository destinationRepository,
        IRecommendationStrategy recommendationStrategy,
        IAiProvider aiProvider,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _destinationRepository = destinationRepository;
        _recommendationStrategy = recommendationStrategy;
        _aiProvider = aiProvider;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<ConversationalRecommendationResponse>> Handle(
        RequestTravelRecommendationsCommand request,
        CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation is null)
        {
            return Result.Failure<ConversationalRecommendationResponse>(
                Error.NotFound("Conversation.NotFound", $"Conversation '{request.ConversationId}' was not found."));
        }

        if (_currentUser.UserId.HasValue && conversation.UserId != _currentUser.UserId.Value)
        {
            return Result.Failure<ConversationalRecommendationResponse>(
                Error.Unauthorized("Conversation.Forbidden", "You do not have permission to access this conversation."));
        }

        var limit = Math.Clamp(request.Limit, 1, 10);
        var collected = new List<Destination>();
        var seenIds = new HashSet<Guid>();

        // 1. Check strategy recommendations if user is authenticated
        if (_currentUser.UserId.HasValue)
        {
            var recommendedIds = await _recommendationStrategy.RecommendAsync(
                _currentUser.UserId.Value,
                limit * 2,
                cancellationToken);

            foreach (var id in recommendedIds)
            {
                if (seenIds.Add(id))
                {
                    var dest = await _destinationRepository.GetByIdAsync(id, cancellationToken);
                    if (dest is not null && MatchesFilter(dest, request.CityId, request.CategoryId))
                    {
                        collected.Add(dest);
                    }
                }
            }
        }

        // 2. Fall back / supplement with catalog search
        if (collected.Count < limit)
        {
            var searchCandidates = await _destinationRepository.SearchAsync(
                searchTerm: null,
                status: DestinationStatus.Published,
                skip: 0,
                take: limit * 3,
                cancellationToken: cancellationToken);

            var filtered = searchCandidates
                .Where(d => MatchesFilter(d, request.CityId, request.CategoryId))
                .Where(d => seenIds.Add(d.Id))
                .OrderByDescending(d => d.PopularityScore)
                .ThenByDescending(d => d.AverageRating)
                .Take(limit - collected.Count);

            collected.AddRange(filtered);
        }

        // 3. Format structured recommendations
        var recommendedItems = collected.Select(d =>
        {
            var translation = d.GetTranslation();
            var coverImage = d.Images.FirstOrDefault(i => i.IsCover)?.Url ?? d.Images.FirstOrDefault()?.Url;
            var reason = d.ReviewCount > 0
                ? $"Top-rated destination with {d.AverageRating:F1}★ from {d.ReviewCount} reviews"
                : "Popular choice among travelers";

            return new ConversationalRecommendationItem(
                d.Id,
                translation.Name,
                translation.Description,
                coverImage,
                d.AverageRating,
                d.ReviewCount,
                reason);
        }).ToList();

        // 4. Construct prompt for local Ollama LLM to formulate conversational pitch
        var userInquiry = string.IsNullOrWhiteSpace(request.Prompt)
            ? "Can you recommend some great places to visit?"
            : request.Prompt.Trim();

        var promptBuilder = new StringBuilder();
        var systemInstruction = conversation.Language.StartsWith("ar", StringComparison.OrdinalIgnoreCase)
            ? "أنت مساعد السفر الذكي لتطبيق فويارا. قدم توصيات سياحية ممتعة وجذابة باللغة العربية بناءً على الأماكن المحددة أدناه."
            : "You are the Voyara AI Travel Assistant. Provide a friendly, inspiring, and concise recommendation pitch introducing the suggested destinations.";

        promptBuilder.AppendLine($"[System: {systemInstruction}]");
        promptBuilder.AppendLine($"User Inquiry: \"{userInquiry}\"");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("Verified Catalog Recommendations:");

        foreach (var item in recommendedItems)
        {
            promptBuilder.AppendLine($"- {item.Name}: {item.Description}. Rating: {item.Rating:F1}★ ({item.ReviewCount} reviews)");
        }

        promptBuilder.AppendLine();
        promptBuilder.AppendLine("Please present these recommendations enthusiastically to the traveler, explaining why each is worth visiting.");
        promptBuilder.Append("Assistant: ");

        var aiPitch = await _aiProvider.GetCompletionAsync(promptBuilder.ToString(), cancellationToken);

        // 5. Record conversational turns
        conversation.AddMessage(MessageRole.User, userInquiry);
        conversation.AddMessage(MessageRole.Assistant, aiPitch);

        _conversationRepository.Update(conversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ConversationalRecommendationResponse(
            conversation.Id,
            aiPitch,
            recommendedItems);
    }

    private static bool MatchesFilter(Destination d, Guid? cityId, Guid? categoryId)
    {
        if (cityId.HasValue && d.CityId != cityId.Value)
        {
            return false;
        }

        if (categoryId.HasValue && d.CategoryId != categoryId.Value)
        {
            return false;
        }

        return true;
    }
}
