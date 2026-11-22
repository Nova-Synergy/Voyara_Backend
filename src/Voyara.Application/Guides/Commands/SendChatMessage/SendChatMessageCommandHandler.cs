using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.SendChatMessage;

public sealed class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, Result<ChatMessageDto>>
{
    private readonly IGuideRequestRepository _requests;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _dateTime;
    private readonly IUnitOfWork _unitOfWork;

    public SendChatMessageCommandHandler(
        IGuideRequestRepository requests,
        ICurrentUser currentUser,
        IDateTime dateTime,
        IUnitOfWork unitOfWork)
    {
        _requests = requests;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ChatMessageDto>> Handle(SendChatMessageCommand command, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure<ChatMessageDto>(GuideErrors.NotAuthenticated);

        var request = await _requests.GetByChatThreadIdAsync(command.ThreadId, null, cancellationToken);
        var thread = request?.ChatThreads.FirstOrDefault(t => t.Id == command.ThreadId);
        if (request is null || thread is null)
            return Result.Failure<ChatMessageDto>(GuideRequestErrors.ChatThreadNotFound);

        var guideUserId = request.GuideProfile!.UserId;
        if (userId != request.UserId && userId != guideUserId)
            return Result.Failure<ChatMessageDto>(GuideRequestErrors.NotChatParticipant);

        if (thread.IsClosed)
            return Result.Failure<ChatMessageDto>(GuideRequestErrors.ChatThreadClosed);

        ChatMessage? message = null;
        var post = GuideDomainCall.Execute(() =>
        {
            message = thread.PostMessage(userId, command.Text, request.UserId, guideUserId, _dateTime.UtcNow);
        });

        if (post.IsFailure)
            return Result.Failure<ChatMessageDto>(post.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChatMessageDto(
            message!.Id, message.ChatThreadId, message.UserId, message.Text, message.SentAt, message.ReadAt);
    }
}
