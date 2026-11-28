using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.AiAssistant;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant.Queries.GetMyConversations;

/// <summary>Returns one page of the current user's conversations.</summary>
public sealed class GetMyConversationsQueryHandler
    : IRequestHandler<GetMyConversationsQuery, Result<PagedResult<ConversationSummaryResponse>>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUser _currentUser;

    /// <summary>Creates the handler.</summary>
    public GetMyConversationsQueryHandler(
        IConversationRepository conversationRepository,
        ICurrentUser currentUser)
    {
        _conversationRepository = conversationRepository;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<ConversationSummaryResponse>>> Handle(
        GetMyConversationsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Result.Failure<PagedResult<ConversationSummaryResponse>>(AiAssistantErrors.UserNotAuthenticated);
        }

        // Page and PageSize are range-checked by GetMyConversationsQueryValidator before the handler runs.
        var skip = (request.Page - 1) * request.PageSize;

        var conversations = await _conversationRepository.ListByUserAsync(
            userId, request.State, skip, request.PageSize, cancellationToken);
        var totalCount = await _conversationRepository.CountByUserAsync(userId, request.State, cancellationToken);

        var messageCounts = await _conversationRepository.GetMessageCountsAsync(
            conversations.Select(conversation => conversation.Id).ToList(),
            cancellationToken);

        var items = conversations
            .Select(conversation => conversation.ToSummary(
                messageCounts.TryGetValue(conversation.Id, out var count) ? count : 0))
            .ToList();

        return new PagedResult<ConversationSummaryResponse>(items, totalCount, request.Page, request.PageSize);
    }
}
