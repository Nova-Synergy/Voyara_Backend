using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.RejectGuideRequest;

public sealed class RejectGuideRequestCommandHandler
    : IRequestHandler<RejectGuideRequestCommand, Result<GuideRequestDetailsDto>>
{
    private readonly IGuideProfileRepository _profiles;
    private readonly IGuideRequestRepository _requests;
    private readonly IGuideRequestReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public RejectGuideRequestCommandHandler(
        IGuideProfileRepository profiles,
        IGuideRequestRepository requests,
        IGuideRequestReadRepository readRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _profiles = profiles;
        _requests = requests;
        _readRepository = readRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GuideRequestDetailsDto>> Handle(
        RejectGuideRequestCommand command, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure<GuideRequestDetailsDto>(GuideErrors.NotAuthenticated);

        var loaded = await GuideRequestWorkflow.LoadForGuideAsync(
            command.RequestId, userId, _profiles, _requests, lockGuide: false, cancellationToken);
        if (loaded.IsFailure)
            return Result.Failure<GuideRequestDetailsDto>(loaded.Error);

        var request = loaded.Value.Request;
        var reject = GuideDomainCall.Execute(() => request.Reject(command.Reason));
        if (reject.IsFailure)
            return Result.Failure<GuideRequestDetailsDto>(reject.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GuideRequestWorkflow.ReadAsync(_readRepository, request.Id, cancellationToken);
    }
}
