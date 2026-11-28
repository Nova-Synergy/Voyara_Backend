using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.AiAssistant.Services;
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
    private readonly IAiContextService _aiContextService;
    private readonly IAiProvider _aiProvider;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="SendMessageToAssistantCommandHandler"/>.
    /// </summary>
    public SendMessageToAssistantCommandHandler(
        IConversationRepository conversationRepository,
        IAiContextService aiContextService,
        IAiProvider aiProvider,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _aiContextService = aiContextService;
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

        if (conversation.State == ConversationState.Completed)
        {
            return Result.Failure<MessageResponse>(
                Error.Conflict("Conversation.Completed", "Cannot send messages to a completed conversation session."));
        }

        // 1. Record user turn
        conversation.AddMessage(MessageRole.User, request.Content.Trim());

        // 2. Build multi-turn contextual prompt enriched with requirements and catalog destinations
        var context = await _aiContextService.BuildContextAsync(
            conversation,
            searchHint: request.Content.Trim(),
            maxDestinations: 5,
            cancellationToken: cancellationToken);
        var prompt = _aiContextService.FormatPrompt(conversation, context, recentMessageCount: 8);

        // 3. Request and validate LLM completion
        var completionResult = await AiResponseHelper.ExecuteCompletionAsync(_aiProvider, prompt, cancellationToken);
        if (completionResult.IsFailure)
        {
            return Result.Failure<MessageResponse>(completionResult.Error);
        }

        // 4. Record assistant turn with estimated token usage
        var (assistantContent, tokensUsed) = completionResult.Value;
        var assistantMessage = conversation.AddMessage(MessageRole.Assistant, assistantContent, tokensUsed);

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
