using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.MarkChatMessageAsRead;

public sealed class MarkChatMessageAsReadCommandHandler
    : IRequestHandler<MarkChatMessageAsReadCommand, Result<ChatMessageDto>>
{
    private readonly IGuideRequestRepository _requests;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _dateTime;
    private readonly IUnitOfWork _unitOfWork;

    public MarkChatMessageAsReadCommandHandler(
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

    public async Task<Result<ChatMessageDto>> Handle(
        MarkChatMessageAsReadCommand command, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure<ChatMessageDto>(GuideErrors.NotAuthenticated);

        var request = await _requests.GetByChatThreadIdAsync(command.ThreadId, command.MessageId, cancellationToken);
        var thread = request?.ChatThreads.FirstOrDefault(t => t.Id == command.ThreadId);
        if (request is null || thread is null)
            return Result.Failure<ChatMessageDto>(GuideRequestErrors.ChatThreadNotFound);

        var guideUserId = request.GuideProfile!.UserId;
        if (userId != request.UserId && userId != guideUserId)
            return Result.Failure<ChatMessageDto>(GuideRequestErrors.NotChatParticipant);

        var message = thread.Messages.FirstOrDefault(m => m.Id == command.MessageId);
        if (message is null)
            return Result.Failure<ChatMessageDto>(GuideRequestErrors.MessageNotFound);

        var read = GuideDomainCall.Execute(() =>
            thread.MarkMessageAsRead(command.MessageId, userId, request.UserId, guideUserId, _dateTime.UtcNow));
        if (read.IsFailure)
            return Result.Failure<ChatMessageDto>(read.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChatMessageDto(
            message.Id, message.ChatThreadId, message.UserId, message.Text, message.SentAt, message.ReadAt);
    }
}
