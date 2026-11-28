using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.AiAssistant;
using Voyara.Domain.Repositories.AiAssistant;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant.Commands.StartConversation;

/// <summary>Creates an Active conversation owned by the current user.</summary>
public sealed class StartConversationCommandHandler : IRequestHandler<StartConversationCommand, Result<Guid>>
{
    private const string DefaultLanguage = "en";

    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public StartConversationCommandHandler(
        IConversationRepository conversationRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> Handle(StartConversationCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Result.Failure<Guid>(AiAssistantErrors.UserNotAuthenticated);
        }

        var language = string.IsNullOrWhiteSpace(request.Language)
            ? DefaultLanguage
            : request.Language.Trim().ToLowerInvariant();

        var conversation = Conversation.Create(userId, language);
        _conversationRepository.Add(conversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return conversation.Id;
    }
}
