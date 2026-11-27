using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.CancelGuideRequest;

public sealed class CancelGuideRequestCommandHandler
    : IRequestHandler<CancelGuideRequestCommand, Result<GuideRequestDetailsDto>>
{
    private readonly IGuideRequestRepository _requests;
    private readonly IGuideRequestReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CancelGuideRequestCommandHandler(
        IGuideRequestRepository requests,
        IGuideRequestReadRepository readRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _requests = requests;
        _readRepository = readRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GuideRequestDetailsDto>> Handle(
        CancelGuideRequestCommand command, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure<GuideRequestDetailsDto>(GuideErrors.NotAuthenticated);

        var request = await _requests.GetByIdAsync(command.RequestId, cancellationToken);
        if (request is null)
            return Result.Failure<GuideRequestDetailsDto>(GuideRequestErrors.RequestNotFound);

        if (request.UserId != userId)
            return Result.Failure<GuideRequestDetailsDto>(GuideRequestErrors.NotRequestOwner);

        // GuideRequest.Cancel enforces which statuses can be cancelled and closes the loaded chat threads.
        var cancel = GuideDomainCall.Execute(request.Cancel);
        if (cancel.IsFailure)
            return Result.Failure<GuideRequestDetailsDto>(cancel.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GuideRequestWorkflow.ReadAsync(_readRepository, request.Id, cancellationToken);
    }
}
