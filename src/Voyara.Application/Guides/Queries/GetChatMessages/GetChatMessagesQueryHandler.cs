using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Queries.GetChatMessages;

public sealed class GetChatMessagesQueryHandler
    : IRequestHandler<GetChatMessagesQuery, Result<PagedResult<ChatMessageDto>>>
{
    private readonly IGuideRequestReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;

    public GetChatMessagesQueryHandler(IGuideRequestReadRepository readRepository, ICurrentUser currentUser)
    {
        _readRepository = readRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<ChatMessageDto>>> Handle(
        GetChatMessagesQuery query, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure<PagedResult<ChatMessageDto>>(GuideErrors.NotAuthenticated);

        var thread = await _readRepository.GetChatThreadAsync(query.ThreadId, cancellationToken);
        if (thread is null)
            return Result.Failure<PagedResult<ChatMessageDto>>(GuideRequestErrors.ChatThreadNotFound);

        if (userId != thread.TouristUserId && userId != thread.GuideUserId)
            return Result.Failure<PagedResult<ChatMessageDto>>(GuideRequestErrors.NotChatParticipant);

        return await _readRepository.ListMessagesAsync(query.ThreadId, query.Page, query.PageSize, cancellationToken);
    }
}
