using MediatR;
using Voyara.Application.Abstractions;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant.Commands.ArchiveConversation;

/// <summary>
/// Handles archiving a conversation session for historical retention.
/// </summary>
public sealed class ArchiveConversationCommandHandler : IRequestHandler<ArchiveConversationCommand, Result>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="ArchiveConversationCommandHandler"/>.
    /// </summary>
    public ArchiveConversationCommandHandler(
        IConversationRepository conversationRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(ArchiveConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation is null)
        {
            return Result.Failure(
                Error.NotFound("Conversation.NotFound", $"Conversation '{request.ConversationId}' was not found."));
        }

        if (_currentUser.UserId.HasValue && conversation.UserId != _currentUser.UserId.Value)
        {
            return Result.Failure(
                Error.Unauthorized("Conversation.Forbidden", "You do not have permission to archive this conversation."));
        }

        conversation.Archive();
        _conversationRepository.Update(conversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
