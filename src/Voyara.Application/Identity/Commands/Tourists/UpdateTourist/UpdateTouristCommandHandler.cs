using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.Tourists.UpdateTourist;

public sealed class UpdateTouristCommandHandler
    : IRequestHandler<UpdateTouristCommand, Result<TouristReadDto>>
{
    private readonly ITouristProfileRepository _tourists;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateTouristCommandHandler(
        ITouristProfileRepository tourists,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _tourists = tourists;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<TouristReadDto>> Handle(
        UpdateTouristCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole(Roles.Admin)
            && _currentUser.UserId != request.UserId)
        {
            return Result.Failure<TouristReadDto>(
                Error.Unauthorized(
                    "Identity.Forbidden",
                    "You are not allowed to access this account."));
        }

        var profile = await _tourists.GetByUserIdAsync(
            request.UserId,
            asNoTracking: false,
            cancellationToken);

        if (profile is null)
        {
            return Result.Failure<TouristReadDto>(
                Error.NotFound(
                    "Identity.NotFound",
                    "Tourist was not found."));
        }

        try
        {
            profile.User.Rename(request.FullName);
            profile.ChangeNationality(request.NationalityCode);
            profile.ChangePreferredLanguage(request.PreferredLanguage);
            profile.ChangeBudget(request.Budget);
            profile.ChangePhoto(request.PhotoUrl);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<TouristReadDto>(
                Error.Conflict("Identity.InvalidState", ex.Message));
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<TouristReadDto>(
                Error.Validation("Identity.InvalidArgument", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TouristReadDto.FromEntity(profile);
    }
}