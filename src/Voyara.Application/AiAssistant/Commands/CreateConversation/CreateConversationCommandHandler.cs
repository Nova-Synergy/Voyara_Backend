using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Entities.AiAssistant;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant.Commands.CreateConversation;

/// <summary>
/// Handles initializing a new AI assistant conversation session and seeding the initial greeting.
/// </summary>
public sealed class CreateConversationCommandHandler
    : IRequestHandler<CreateConversationCommand, Result<ConversationDetailResponse>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="CreateConversationCommandHandler"/>.
    /// </summary>
    public CreateConversationCommandHandler(
        IConversationRepository conversationRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<ConversationDetailResponse>> Handle(
        CreateConversationCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? Guid.NewGuid();
        var language = string.IsNullOrWhiteSpace(request.Language) ? "en" : request.Language.Trim().ToLowerInvariant();

        var conversation = Conversation.Create(userId, language);

        var greeting = language.StartsWith("ar", StringComparison.OrdinalIgnoreCase)
            ? "مرحباً بك! أنا مساعد السفر الذكي الخاص بك في فويارا. كيف يمكنني مساعدتك في تخطيط رحلتك اليوم؟"
            : "Hello! I am your Voyara AI Travel Assistant. How can I help you plan your journey today?";

        conversation.AddMessage(MessageRole.Assistant, greeting);

        _conversationRepository.Add(conversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
