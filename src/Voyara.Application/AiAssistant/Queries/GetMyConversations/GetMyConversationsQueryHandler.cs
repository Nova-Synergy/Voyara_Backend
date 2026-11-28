using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant.Queries.GetMyConversations;

/// <summary>
/// Handles listing conversation sessions belonging to the calling user with pagination.
/// </summary>
public sealed class GetMyConversationsQueryHandler
    : IRequestHandler<GetMyConversationsQuery, Result<PagedResult<ConversationSummaryResponse>>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of <see cref="GetMyConversationsQueryHandler"/>.
    /// </summary>
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
        var userId = _currentUser.UserId ?? Guid.Empty;
        if (userId == Guid.Empty)
        {
            return Result.Failure<PagedResult<ConversationSummaryResponse>>(
                Error.Unauthorized("User.Unauthorized", "User must be authenticated to view conversations."));
        }

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        var conversations = await _conversationRepository.GetPagedByUserIdAsync(
            userId, skip, pageSize, cancellationToken);
        var totalCount = await _conversationRepository.CountByUserIdAsync(userId, cancellationToken);

        var summaries = conversations.Select(c => new ConversationSummaryResponse(
            c.Id,
            c.Language,
            c.State.ToString(),
            c.StartedAt,
            c.Messages.Count,
            c.JourneyId)).ToList();

        return new PagedResult<ConversationSummaryResponse>(summaries, totalCount, page, pageSize);
    }
}
