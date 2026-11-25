using System.Text;
using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant.Commands.SendMessage;

/// <summary>
/// Handles sending a user prompt to the local Ollama LLM assistant and recording conversational turns.
/// </summary>
public sealed class SendMessageToAssistantCommandHandler
    : IRequestHandler<SendMessageToAssistantCommand, Result<MessageResponse>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IAiProvider _aiProvider;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="SendMessageToAssistantCommandHandler"/>.
    /// </summary>
    public SendMessageToAssistantCommandHandler(
        IConversationRepository conversationRepository,
        IAiProvider aiProvider,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _aiProvider = aiProvider;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<MessageResponse>> Handle(
        SendMessageToAssistantCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result.Failure<MessageResponse>(
                Error.Validation("Message.Empty", "Message content cannot be empty."));
        }

        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation is null)
        {
            return Result.Failure<MessageResponse>(
                Error.NotFound("Conversation.NotFound", $"Conversation '{request.ConversationId}' was not found."));
        }

        if (_currentUser.UserId.HasValue && conversation.UserId != _currentUser.UserId.Value)
        {
            return Result.Failure<MessageResponse>(
                Error.Unauthorized("Conversation.Forbidden", "You do not have permission to access this conversation."));
        }

        if (conversation.State == ConversationState.Archived)
        {
            return Result.Failure<MessageResponse>(
                Error.Conflict("Conversation.Archived", "Cannot send messages to an archived conversation session."));
        }

        // 1. Record user turn
        conversation.AddMessage(MessageRole.User, request.Content.Trim());

        // 2. Build multi-turn contextual prompt for local Ollama LLM
        var promptBuilder = new StringBuilder();
        var systemInstruction = conversation.Language.StartsWith("ar", StringComparison.OrdinalIgnoreCase)
            ? "أنت مساعد السفر الذكي لتطبيق فويارا (Voyara). ساعد المستخدم في تخطيط الرحلات واقتراح الوجهات السياحية والإجابة بدقة ولطف باللغة العربية."
            : "You are the Voyara AI Travel Assistant. Help the tourist plan their trip, suggest attractions and destinations, and answer travel questions concisely and helpfully.";

        promptBuilder.AppendLine($"[System: {systemInstruction}]");
        promptBuilder.AppendLine();

        // Include recent history (last 8 messages) for conversational memory
        var recentHistory = conversation.Messages.TakeLast(8);
        foreach (var msg in recentHistory)
        {
            promptBuilder.AppendLine($"{msg.Role}: {msg.Content}");
        }

        promptBuilder.Append("Assistant: ");

        // 3. Request LLM completion via Ollama provider
        var assistantCompletion = await _aiProvider.GetCompletionAsync(promptBuilder.ToString(), cancellationToken);

        // 4. Record assistant turn
        var assistantMessage = conversation.AddMessage(MessageRole.Assistant, assistantCompletion);

        _conversationRepository.Update(conversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MessageResponse(
            assistantMessage.Id,
            assistantMessage.Role.ToString(),
            assistantMessage.Content,
            assistantMessage.TokensUsed,
            assistantMessage.CreatedAt);
    }
}
