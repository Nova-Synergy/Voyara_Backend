using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.TourGuides.DeleteTourGuide;

public sealed class DeleteTourGuideCommandHandler
    : IRequestHandler<DeleteTourGuideCommand, Result>
{
    private readonly IGuideProfileRepository _guides;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public DeleteTourGuideCommandHandler(
        IGuideProfileRepository guides,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _guides = guides;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        DeleteTourGuideCommand request,
        CancellationToken cancellationToken)
    {
        var profile = await _guides.GetByIdAsync(
            request.Id,
            asNoTracking: false,
            cancellationToken);

        if (profile is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Identity.NotFound",
                    "Tour guide was not found."));
        }

        if (!_currentUser.IsInRole(Roles.Admin)
            && _currentUser.UserId != profile.UserId)
        {
            return Result.Failure(
                Error.Unauthorized(
                    "Identity.Forbidden",
                    "You are not allowed to access this account."));
        }

        profile.IsDeleted = true;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}