using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant.Queries.GetMyConversations;

/// <summary>
/// Handles listing all conversation sessions belonging to the calling user.
/// </summary>
public sealed class GetMyConversationsQueryHandler
    : IRequestHandler<GetMyConversationsQuery, Result<IReadOnlyList<ConversationSummaryResponse>>>
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
    public async Task<Result<IReadOnlyList<ConversationSummaryResponse>>> Handle(
        GetMyConversationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        if (userId == Guid.Empty)
        {
            return Result.Failure<IReadOnlyList<ConversationSummaryResponse>>(
                Error.Unauthorized("User.Unauthorized", "User must be authenticated to view conversations."));
        }

        var conversations = await _conversationRepository.GetByUserIdAsync(userId, cancellationToken);

        var summaries = conversations.Select(c => new ConversationSummaryResponse(
            c.Id,
            c.Language,
            c.State.ToString(),
            c.StartedAt,
            c.Messages.Count,
            c.JourneyId)).ToList();

        return summaries;
    }
}
