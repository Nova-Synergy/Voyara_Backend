using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.AiAssistant.Services;
using Voyara.Application.DTOs;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant.Queries.GetConversationContext;

/// <summary>
/// Handles assembling AI conversation context integrated with published catalog destinations.
/// </summary>
public sealed class GetConversationContextQueryHandler
    : IRequestHandler<GetConversationContextQuery, Result<AiConversationContextDto>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IAiContextService _aiContextService;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of <see cref="GetConversationContextQueryHandler"/>.
    /// </summary>
    public GetConversationContextQueryHandler(
        IConversationRepository conversationRepository,
        IAiContextService aiContextService,
        ICurrentUser currentUser)
    {
        _conversationRepository = conversationRepository;
        _aiContextService = aiContextService;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<AiConversationContextDto>> Handle(
        GetConversationContextQuery request,
        CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation is null)
        {
            return Result.Failure<AiConversationContextDto>(
                Error.NotFound("Conversation.NotFound", $"Conversation '{request.ConversationId}' was not found."));
        }

        if (_currentUser.UserId.HasValue && conversation.UserId != _currentUser.UserId.Value)
        {
            return Result.Failure<AiConversationContextDto>(
                Error.Unauthorized("Conversation.Forbidden", "You do not have permission to access this conversation."));
        }

        return await _aiContextService.BuildContextAsync(
            conversation,
            request.SearchHint,
            request.MaxDestinations,
            cancellationToken);
    }
}
