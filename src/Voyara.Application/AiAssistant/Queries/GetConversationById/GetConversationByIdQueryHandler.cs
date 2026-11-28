using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.AiAssistant;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant.Queries.GetConversationById;

/// <summary>Returns a conversation owned by the current user.</summary>
public sealed class GetConversationByIdQueryHandler
    : IRequestHandler<GetConversationByIdQuery, Result<ConversationDetailsResponse>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUser _currentUser;

    /// <summary>Creates the handler.</summary>
    public GetConversationByIdQueryHandler(
        IConversationRepository conversationRepository,
        ICurrentUser currentUser)
    {
        _conversationRepository = conversationRepository;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<ConversationDetailsResponse>> Handle(
        GetConversationByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Result.Failure<ConversationDetailsResponse>(AiAssistantErrors.UserNotAuthenticated);
        }

        var conversation = await _conversationRepository.GetByIdWithMessagesAsync(
            request.ConversationId, cancellationToken);

        // Someone else's conversation is reported as not found so its existence is not disclosed.
        if (conversation is null || conversation.UserId != userId)
        {
            return Result.Failure<ConversationDetailsResponse>(AiAssistantErrors.NotFound(request.ConversationId));
        }

        return conversation.ToDetails();
    }
}
