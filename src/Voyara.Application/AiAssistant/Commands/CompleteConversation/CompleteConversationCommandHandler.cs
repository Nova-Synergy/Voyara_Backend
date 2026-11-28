using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.AiAssistant;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant.Commands.CompleteConversation;

/// <summary>Completes a conversation owned by the current user.</summary>
public sealed class CompleteConversationCommandHandler : IRequestHandler<CompleteConversationCommand, Result>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public CompleteConversationCommandHandler(
        IConversationRepository conversationRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(CompleteConversationCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Result.Failure(AiAssistantErrors.UserNotAuthenticated);
        }

        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);

        // Someone else's conversation is reported as not found so its existence is not disclosed.
        if (conversation is null || conversation.UserId != userId)
        {
            return Result.Failure(AiAssistantErrors.NotFound(request.ConversationId));
        }

        // The aggregate silently ignores Complete() on an archived conversation; report it instead.
        if (conversation.State == ConversationState.Archived)
        {
            return Result.Failure(AiAssistantErrors.Archived);
        }

        if (conversation.State == ConversationState.Completed)
        {
            return Result.Success();
        }

        conversation.Complete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
