using MediatR;
using Voyara.Application.Abstractions;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Queries.GetChatThread;

public sealed class GetChatThreadQueryHandler : IRequestHandler<GetChatThreadQuery, Result<ChatThreadDto>>
{
    private readonly IGuideRequestReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;

    public GetChatThreadQueryHandler(IGuideRequestReadRepository readRepository, ICurrentUser currentUser)
    {
        _readRepository = readRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<ChatThreadDto>> Handle(GetChatThreadQuery query, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure<ChatThreadDto>(GuideErrors.NotAuthenticated);

        var thread = await _readRepository.GetChatThreadAsync(query.ThreadId, cancellationToken);
        if (thread is null)
            return Result.Failure<ChatThreadDto>(GuideRequestErrors.ChatThreadNotFound);

        if (userId != thread.TouristUserId && userId != thread.GuideUserId)
            return Result.Failure<ChatThreadDto>(GuideRequestErrors.NotChatParticipant);

        return new ChatThreadDto(thread.Id, thread.GuideRequestId, thread.IsClosed);
    }
}
