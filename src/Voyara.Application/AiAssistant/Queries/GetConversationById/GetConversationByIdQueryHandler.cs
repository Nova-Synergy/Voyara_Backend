using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant.Queries.GetConversationById;

/// <summary>
/// Handles retrieving conversation details and message history.
/// </summary>
public sealed class GetConversationByIdQueryHandler
    : IRequestHandler<GetConversationByIdQuery, Result<ConversationDetailResponse>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of <see cref="GetConversationByIdQueryHandler"/>.
    /// </summary>
    public GetConversationByIdQueryHandler(
        IConversationRepository conversationRepository,
        ICurrentUser currentUser)
    {
        _conversationRepository = conversationRepository;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<ConversationDetailResponse>> Handle(
        GetConversationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation is null)
        {
            return Result.Failure<ConversationDetailResponse>(
                Error.NotFound("Conversation.NotFound", $"Conversation '{request.ConversationId}' was not found."));
        }

        if (_currentUser.UserId.HasValue && conversation.UserId != _currentUser.UserId.Value)
        {
            return Result.Failure<ConversationDetailResponse>(
                Error.Unauthorized("Conversation.Forbidden", "You do not have permission to view this conversation."));
        }

        var messages = conversation.Messages
            .OrderBy(m => m.CreatedAt)
            .Select(m => new MessageResponse(m.Id, m.Role.ToString(), m.Content, m.TokensUsed, m.CreatedAt))
            .ToList();

        return new ConversationDetailResponse(
            conversation.Id,
            conversation.Language,
            conversation.State.ToString(),
            conversation.StartedAt,
            conversation.JourneyId,
            messages);
    }
}
