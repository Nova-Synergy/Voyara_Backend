using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.TourGuides.UpdateTourGuide;

public sealed class UpdateTourGuideCommandHandler
    : IRequestHandler<UpdateTourGuideCommand, Result<TourGuideReadDto>>
{
    private readonly IGuideProfileRepository _guides;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateTourGuideCommandHandler(
        IGuideProfileRepository guides,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _guides = guides;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<TourGuideReadDto>> Handle(
        UpdateTourGuideCommand request,
        CancellationToken cancellationToken)
    {
        var profile = await _guides.GetByIdAsync(
            request.Id,
            asNoTracking: false,
            cancellationToken);

        if (profile is null)
        {
            return Result.Failure<TourGuideReadDto>(
                Error.NotFound(
                    "Identity.NotFound",
                    "Tour guide was not found."));
        }

        if (!_currentUser.IsInRole(Roles.Admin)
            && _currentUser.UserId != profile.UserId)
        {
            return Result.Failure<TourGuideReadDto>(
                Error.Unauthorized(
                    "Identity.Forbidden",
                    "You are not allowed to access this account."));
        }

        try
        {
            profile.UpdateDetails(
                request.Bio,
                request.PhotoUrl,
                request.YearsOfExperience);

            profile.User!.Rename(request.FullName);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<TourGuideReadDto>(
                Error.Conflict("Identity.InvalidState", ex.Message));
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<TourGuideReadDto>(
                Error.Validation("Identity.InvalidArgument", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TourGuideReadDto.FromEntity(profile);
    }
}