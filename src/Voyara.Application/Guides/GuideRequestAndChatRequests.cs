using Voyara.Api.DTOs.Guides;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides;

/// <summary>
/// Represents a query to retrieve a paginated list of guide profiles.
/// </summary>
/// <param name="Page">The requested page number. Defaults to 1.</param>
/// <param name="PageSize">The maximum number of profiles per page. Defaults to 10.</param>
public sealed record GetGuideProfilesQuery(int Page = 1, int PageSize = 10) : IQuery<PagedResult<GuideProfileReadDto>>;

/// <summary>
/// Represents a query to retrieve a guide profile by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the guide profile.</param>
public sealed record GetGuideProfileQuery(Guid Id) : IQuery<GuideProfileDetailDto>;

/// <summary>
/// Represents a command to create a new guide profile for the current user.
/// </summary>
/// <param name="Dto">The data required to create the guide profile.</param>
public sealed record CreateGuideProfileCommand(CreateGuideProfileDto Dto) : ICommand<GuideProfileReadDto>;

/// <summary>
/// Represents a command to update an existing guide profile.
/// </summary>
/// <param name="Id">The unique identifier of the guide profile to update.</param>
/// <param name="Dto">The updated profile information.</param>
public sealed record UpdateGuideProfileCommand(Guid Id, UpdateGuideProfileDto Dto) : ICommand;

/// <summary>
/// Represents a command to delete an existing guide profile.
/// </summary>
/// <param name="Id">The unique identifier of the guide profile to delete.</param>
public sealed record DeleteGuideProfileCommand(Guid Id) : ICommand;

/// <summary>
/// Represents a query to retrieve paginated reviews for a specific guide profile.
/// </summary>
/// <param name="ProfileId">The unique identifier of the guide profile.</param>
/// <param name="Page">The requested page number. Defaults to 1.</param>
/// <param name="PageSize">The maximum number of reviews per page. Defaults to 10.</param>
public sealed record GetGuideReviewsQuery(Guid ProfileId, int Page = 1, int PageSize = 10) : IQuery<PagedResult<GuideReviewReadDto>>;

/// <summary>
/// Represents a query to retrieve paginated requests associated with a guide profile.
/// </summary>
/// <param name="ProfileId">The unique identifier of the guide profile.</param>
/// <param name="Status">An optional status used to filter guide requests.</param>
/// <param name="Page">The requested page number. Defaults to 1.</param>
/// <param name="PageSize">The maximum number of requests per page. Defaults to 10.</param>
public sealed record GetGuideRequestsQuery(Guid ProfileId, RequestStatus? Status, int Page = 1, int PageSize = 10) : IQuery<PagedResult<GuideRequestReadDto>>;

/// <summary>
/// Represents a command to add a language to a guide profile.
/// </summary>
/// <param name="ProfileId">The unique identifier of the guide profile.</param>
/// <param name="Dto">The language information to add.</param>
public sealed record AddGuideLanguageCommand(Guid ProfileId, CreateGuideLanguageDto Dto) : ICommand<GuideLanguageReadDto>;

/// <summary>
/// Represents a command to remove a language from a guide profile.
/// </summary>
/// <param name="ProfileId">The unique identifier of the guide profile.</param>
/// <param name="ItemId">The unique identifier of the language entry to remove.</param>
public sealed record RemoveGuideLanguageCommand(Guid ProfileId, Guid ItemId) : ICommand;

/// <summary>
/// Represents a command to add an area of expertise to a guide profile.
/// </summary>
/// <param name="ProfileId">The unique identifier of the guide profile.</param>
/// <param name="Dto">The expertise information to add.</param>
public sealed record AddGuideExpertiseCommand(Guid ProfileId, CreateGuideExpertiseDto Dto) : ICommand<GuideExpertiseReadDto>;

/// <summary>
/// Represents a command to remove an area of expertise from a guide profile.
/// </summary>
/// <param name="ProfileId">The unique identifier of the guide profile.</param>
/// <param name="ItemId">The unique identifier of the expertise entry to remove.</param>
public sealed record RemoveGuideExpertiseCommand(Guid ProfileId, Guid ItemId) : ICommand;

/// <summary>
/// Represents a command to add an availability period to a guide profile.
/// </summary>
/// <param name="ProfileId">The unique identifier of the guide profile.</param>
/// <param name="Dto">The availability information to add.</param>
public sealed record AddGuideAvailabilityCommand(Guid ProfileId, CreateGuideAvailabilityDto Dto) : ICommand<GuideAvailabilityReadDto>;

/// <summary>
/// Represents a command to remove an availability period from a guide profile.
/// </summary>
/// <param name="ProfileId">The unique identifier of the guide profile.</param>
/// <param name="ItemId">The unique identifier of the availability entry to remove.</param>
public sealed record RemoveGuideAvailabilityCommand(Guid ProfileId, Guid ItemId) : ICommand;

/// <summary>
/// Represents a command to add a time-off date to a guide profile.
/// </summary>
/// <param name="ProfileId">The unique identifier of the guide profile.</param>
/// <param name="Dto">The time-off information to add.</param>
public sealed record AddGuideTimeOffCommand(Guid ProfileId, CreateGuideTimeOffDto Dto) : ICommand<GuideTimeOffReadDto>;

/// <summary>
/// Represents a command to remove a time-off entry from a guide profile.
/// </summary>
/// <param name="ProfileId">The unique identifier of the guide profile.</param>
/// <param name="ItemId">The unique identifier of the time-off entry to remove.</param>
public sealed record RemoveGuideTimeOffCommand(Guid ProfileId, Guid ItemId) : ICommand;

/// <summary>
/// Represents a command to add a pricing option to a guide profile.
/// </summary>
/// <param name="ProfileId">The unique identifier of the guide profile.</param>
/// <param name="Dto">The pricing information to add.</param>
public sealed record AddGuidePricingCommand(Guid ProfileId, CreateGuidePricingDto Dto) : ICommand<GuidePricingReadDto>;

/// <summary>
/// Represents a command to remove a pricing option from a guide profile.
/// </summary>
/// <param name="ProfileId">The unique identifier of the guide profile.</param>
/// <param name="ItemId">The unique identifier of the pricing entry to remove.</param>
public sealed record RemoveGuidePricingCommand(Guid ProfileId, Guid ItemId) : ICommand;

/// <summary>
/// Handles requests to retrieve paginated guide profiles.
/// </summary>
/// <param name="profiles">The repository used to retrieve guide profiles.</param>
public sealed class GetGuideProfilesHandler(IGuideProfileRepository profiles) : MediatR.IRequestHandler<GetGuideProfilesQuery, Result<PagedResult<GuideProfileReadDto>>>
{
    /// <summary>
    /// Retrieves a paginated list of guide profiles and their total count.
    /// </summary>
    /// <param name="request">The query containing pagination parameters.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A result containing the requested page of guide profiles.</returns>
    public async Task<Result<PagedResult<GuideProfileReadDto>>> Handle(GetGuideProfilesQuery request, CancellationToken ct)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        var profilesPage = await profiles.GetAllAsync(skip, pageSize, ct);
        var totalCount = await profiles.CountAsync(ct);
        var items = profilesPage.Select(GuideProfileReadDto.FromEntity).ToList();

        return new PagedResult<GuideProfileReadDto>(items, totalCount, page, pageSize);
    }
}

/// <summary>
/// Handles requests to retrieve a guide profile with its related details.
/// </summary>
/// <param name="profiles">The repository used to retrieve guide profiles.</param>
public sealed class GetGuideProfileHandler(IGuideProfileRepository profiles) : MediatR.IRequestHandler<GetGuideProfileQuery, Result<GuideProfileDetailDto>>
{
    /// <summary>
    /// Retrieves a guide profile by its unique identifier, including its details.
    /// </summary>
    /// <param name="request">The query containing the profile identifier.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The requested guide profile, or a not-found error if it does not exist.</returns>
    public async Task<Result<GuideProfileDetailDto>> Handle(GetGuideProfileQuery request, CancellationToken ct)
    {
        var profile = await profiles.GetByIdWithDetailsAsync(request.Id, ct);
        return profile is null ? GuideErrors.NotFound<GuideProfileDetailDto>("GuideProfile") : GuideProfileDetailDto.FromEntity(profile);
    }
}

/// <summary>
/// Handles the creation of guide profiles.
/// </summary>
/// <param name="profiles">The repository used to manage guide profiles.</param>
/// <param name="currentUser">Provides information about the currently authenticated user.</param>
/// <param name="unitOfWork">Coordinates persistence operations.</param>
public sealed class CreateGuideProfileHandler(IGuideProfileRepository profiles, ICurrentUser currentUser, IUnitOfWork unitOfWork) : MediatR.IRequestHandler<CreateGuideProfileCommand, Result<GuideProfileReadDto>>
{
    /// <summary>
    /// Creates a guide profile for the currently authenticated user.
    /// </summary>
    /// <param name="request">The command containing the profile creation data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The created guide profile, or an appropriate error if creation fails.</returns>
    public async Task<Result<GuideProfileReadDto>> Handle(CreateGuideProfileCommand request, CancellationToken ct)
    {
        if (currentUser.UserId is not { } userId)
            return GuideErrors.Unauthenticated<GuideProfileReadDto>();

        if (!await profiles.CityExistsAsync(request.Dto.CityId, ct))
            return GuideErrors.Validation<GuideProfileReadDto>("The city does not exist.");

        if (await profiles.ExistsForUserAsync(userId, ct))
            return GuideErrors.Conflict<GuideProfileReadDto>("This user already has a guide profile.");

        try
        {
            var profile = GuideProfile.Create
                    (
                        userId,
                        request.Dto.CityId,
                        request.Dto.Bio,
                        request.Dto.PhotoUrl,
                        request.Dto.YearsOfExperience
                    );

            profiles.Add(profile);
            await unitOfWork.SaveChangesAsync(ct);
            return GuideProfileReadDto.FromEntity(profile);
        }
        catch (ArgumentException ex)
        {
            return GuideErrors.Validation<GuideProfileReadDto>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return GuideErrors.Conflict<GuideProfileReadDto>(ex.Message);
        }
    }
}

/// <summary>
/// Handles updates to existing guide profiles.
/// </summary>
/// <param name="profiles">The repository used to retrieve guide profiles.</param>
/// <param name="currentUser">Provides information about the currently authenticated user.</param>
/// <param name="unitOfWork">Coordinates persistence operations.</param>
public sealed class UpdateGuideProfileHandler(IGuideProfileRepository profiles, ICurrentUser currentUser, IUnitOfWork unitOfWork) : MediatR.IRequestHandler<UpdateGuideProfileCommand, Result>
{
    /// <summary>
    /// Updates the details of a guide profile owned by the current user.
    /// </summary>
    /// <param name="request">The command containing the profile identifier and updated data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A successful result or an error describing why the update failed.</returns>
    public async Task<Result> Handle(UpdateGuideProfileCommand request, CancellationToken ct)
    {
        var profile = await profiles.GetByIdWithDetailsAsync(request.Id, ct);

        if (profile is null)
            return GuideErrors.NotFound();

        if (profile.UserId != currentUser.UserId)
            return GuideErrors.Forbidden();

        try
        {
            profile.UpdateDetails
                    (
                        request.Dto.Bio,
                        request.Dto.PhotoUrl,
                        request.Dto.YearsOfExperience
                    );

            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return GuideErrors.Validation(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return GuideErrors.Conflict(ex.Message);
        }
    }
}

/// <summary>
/// Handles deletion of guide profiles.
/// </summary>
/// <param name="profiles">The repository used to manage guide profiles.</param>
/// <param name="currentUser">Provides information about the currently authenticated user.</param>
/// <param name="unitOfWork">Coordinates persistence operations.</param>
public sealed class DeleteGuideProfileHandler(IGuideProfileRepository profiles, ICurrentUser currentUser, IUnitOfWork unitOfWork) : MediatR.IRequestHandler<DeleteGuideProfileCommand, Result>
{
    /// <summary>
    /// Deletes a guide profile owned by the current user.
    /// </summary>
    /// <param name="request">The command containing the profile identifier.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A successful result or an error if the profile does not exist or is not owned by the current user.</returns>
    public async Task<Result> Handle(DeleteGuideProfileCommand request, CancellationToken ct)
    {
        var profile = await profiles.GetByIdWithDetailsAsync(request.Id, ct);

        if (profile is null)
            return GuideErrors.NotFound();

        if (profile.UserId != currentUser.UserId)
            return GuideErrors.Forbidden();

        profiles.Remove(profile);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}

/// <summary>
/// Handles requests to retrieve paginated reviews for a guide.
/// </summary>
/// <param name="profiles">The repository used to check guide profile existence.</param>
/// <param name="requests">The repository used to retrieve guide reviews.</param>
public sealed class GetGuideReviewsHandler(IGuideProfileRepository profiles, IGuideRequestRepository requests) : MediatR.IRequestHandler<GetGuideReviewsQuery, Result<PagedResult<GuideReviewReadDto>>>
{
    /// <summary>
    /// Retrieves reviews associated with a guide profile.
    /// </summary>
    /// <param name="request">The query containing the profile identifier and pagination parameters.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A paginated result containing the guide reviews.</returns>
    public async Task<Result<PagedResult<GuideReviewReadDto>>> Handle(GetGuideReviewsQuery request, CancellationToken ct)
    {
        if (!await profiles.ExistsAsync(request.ProfileId, ct))
            return GuideErrors.NotFound<PagedResult<GuideReviewReadDto>>("GuideProfile");

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        var reviews = await requests.GetReviewsForGuideAsync(request.ProfileId, skip, pageSize, ct);
        var totalCount = await requests.CountReviewsForGuideAsync(request.ProfileId, ct);
        var items = reviews.Select(GuideReviewReadDto.FromEntity).ToList();

        return new PagedResult<GuideReviewReadDto>(items, totalCount, page, pageSize);
    }
}

/// <summary>
/// Handles requests to retrieve guide requests associated with a guide profile.
/// </summary>
/// <param name="profiles">The repository used to retrieve guide profiles.</param>
/// <param name="requests">The repository used to retrieve guide requests.</param>
/// <param name="currentUser">Provides information about the currently authenticated user.</param>
public sealed class GetGuideRequestsHandler(IGuideProfileRepository profiles, IGuideRequestRepository requests, ICurrentUser currentUser) : MediatR.IRequestHandler<GetGuideRequestsQuery, Result<PagedResult<GuideRequestReadDto>>>
{
    /// <summary>
    /// Retrieves paginated requests belonging to a guide profile owned by the current user.
    /// </summary>
    /// <param name="request">The query containing the profile identifier, optional status filter, and pagination parameters.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A paginated result containing the matching guide requests.</returns>
    public async Task<Result<PagedResult<GuideRequestReadDto>>> Handle(GetGuideRequestsQuery request, CancellationToken ct)
    {
        var profile = await profiles.GetByIdAsync(request.ProfileId, ct);

        if (profile is null)
            return GuideErrors.NotFound<PagedResult<GuideRequestReadDto>>("GuideProfile");

        if (profile.UserId != currentUser.UserId)
            return GuideErrors.Forbidden<PagedResult<GuideRequestReadDto>>();

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        var requestsPage = await requests.GetForGuideAsync(request.ProfileId, request.Status, skip, pageSize, ct);
        var totalCount = await requests.CountForGuideAsync(request.ProfileId, request.Status, ct);
        var items = requestsPage.Select(GuideRequestReadDto.FromEntity).ToList();

        return new PagedResult<GuideRequestReadDto>(items, totalCount, page, pageSize);
    }
}

/// <summary>
/// Handles adding languages to guide profiles.
/// </summary>
/// <param name="profiles">The repository used to retrieve guide profiles.</param>
/// <param name="currentUser">Provides information about the currently authenticated user.</param>
/// <param name="unitOfWork">Coordinates persistence operations.</param>
public sealed class AddGuideLanguageHandler(IGuideProfileRepository profiles, ICurrentUser currentUser, IUnitOfWork unitOfWork) : MediatR.IRequestHandler<AddGuideLanguageCommand, Result<GuideLanguageReadDto>>
{
    /// <summary>
    /// Adds a language to a guide profile owned by the current user.
    /// </summary>
    /// <param name="request">The command containing the profile identifier and language data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The added language entry, or an error if the operation fails.</returns>
    public async Task<Result<GuideLanguageReadDto>> Handle(AddGuideLanguageCommand request, CancellationToken ct)
    {
        var loaded = await GuideHandlers.OwnProfile(profiles, currentUser, request.ProfileId, ct);

        if (loaded.Error is not null)
            return Result.Failure<GuideLanguageReadDto>(loaded.Error);

        try
        {
            var ids = loaded.Profile!.Languages.Select(x => x.Id)
                            .ToHashSet();

            loaded.Profile.AddLanguage(request.Dto.LanguageCode, request.Dto.Proficiency);
            await unitOfWork.SaveChangesAsync(ct);
            return GuideLanguageReadDto.FromEntity(loaded.Profile.Languages.First(x => !ids.Contains(x.Id)));
        }

        catch (ArgumentException ex)
        {
            return GuideErrors.Validation<GuideLanguageReadDto>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return GuideErrors.Conflict<GuideLanguageReadDto>(ex.Message);
        }
    }
}

/// <summary>
/// Handles removing languages from guide profiles.
/// </summary>
/// <param name="p">The guide profile repository.</param>
/// <param name="u">Provides information about the current user.</param>
/// <param name="w">Coordinates persistence operations.</param>
public sealed class RemoveGuideLanguageHandler(IGuideProfileRepository p, ICurrentUser u, IUnitOfWork w) : MediatR.IRequestHandler<RemoveGuideLanguageCommand, Result>
{
    /// <summary>
    /// Removes a language entry from a guide profile owned by the current user.
    /// </summary>
    /// <param name="r">The removal command.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A successful result or an error describing why removal failed.</returns>
    public async Task<Result> Handle(RemoveGuideLanguageCommand r, CancellationToken ct)
        => await GuideHandlers.Remove(
            p, u, w, r.ProfileId,
            r.ItemId,
            x => x.Languages.FirstOrDefault(i => i.Id == r.ItemId),
            (x, i) => x.RemoveLanguage(i.LanguageCode), ct
            );
}

/// <summary>
/// Handles adding areas of expertise to guide profiles.
/// </summary>
/// <param name="profiles">The guide profile repository.</param>
/// <param name="currentUser">Provides information about the current user.</param>
/// <param name="unitOfWork">Coordinates persistence operations.</param>
public sealed class AddGuideExpertiseHandler(IGuideProfileRepository profiles, ICurrentUser currentUser, IUnitOfWork unitOfWork) : MediatR.IRequestHandler<AddGuideExpertiseCommand, Result<GuideExpertiseReadDto>>
{
    /// <summary>
    /// Adds an expertise category to a guide profile owned by the current user.
    /// </summary>
    /// <param name="request">The command containing the profile identifier and category data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The added expertise entry, or an error if the operation fails.</returns>
    public async Task<Result<GuideExpertiseReadDto>> Handle(AddGuideExpertiseCommand request, CancellationToken ct)
    {
        var loaded = await GuideHandlers.OwnProfile(profiles, currentUser, request.ProfileId, ct);

        if (loaded.Error is not null)
            return Result.Failure<GuideExpertiseReadDto>(loaded.Error);

        try
        {
            var ids = loaded.Profile!.Expertise.Select(x => x.Id).ToHashSet();
            loaded.Profile.AddExpertise(request.Dto.CategoryId);
            await unitOfWork.SaveChangesAsync(ct);
            return GuideExpertiseReadDto.FromEntity(loaded.Profile.Expertise.First(x => !ids.Contains(x.Id)));
        }
        catch (ArgumentException ex)
        {
            return GuideErrors.Validation<GuideExpertiseReadDto>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return GuideErrors.Conflict<GuideExpertiseReadDto>(ex.Message);
        }
    }
}

/// <summary>
/// Handles removing areas of expertise from guide profiles.
/// </summary>
/// <param name="p">The guide profile repository.</param>
/// <param name="u">Provides information about the current user.</param>
/// <param name="w">Coordinates persistence operations.</param>
public sealed class RemoveGuideExpertiseHandler(IGuideProfileRepository p, ICurrentUser u, IUnitOfWork w) : MediatR.IRequestHandler<RemoveGuideExpertiseCommand, Result>
{
    /// <summary>
    /// Removes an expertise entry from a guide profile.
    /// </summary>
    /// <param name="r">The removal command.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A successful result or an error if removal fails.</returns>
    public async Task<Result> Handle(RemoveGuideExpertiseCommand r, CancellationToken ct)
        => await GuideHandlers.Remove(p, u, w, r.ProfileId, r.ItemId, x => x.Expertise.FirstOrDefault(i => i.Id == r.ItemId), (x, i) => x.RemoveExpertise(i.CategoryId), ct);
}

/// <summary>
/// Handles adding availability periods to guide profiles.
/// </summary>
/// <param name="profiles">The guide profile repository.</param>
/// <param name="currentUser">Provides information about the current user.</param>
/// <param name="unitOfWork">Coordinates persistence operations.</param>
public sealed class AddGuideAvailabilityHandler(IGuideProfileRepository profiles, ICurrentUser currentUser, IUnitOfWork unitOfWork) : MediatR.IRequestHandler<AddGuideAvailabilityCommand, Result<GuideAvailabilityReadDto>>
{
    /// <summary>
    /// Adds an availability period to a guide profile owned by the current user.
    /// </summary>
    /// <param name="r">The command containing the availability data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The added availability entry, or an error if the operation fails.</returns>
    public async Task<Result<GuideAvailabilityReadDto>> Handle(AddGuideAvailabilityCommand r, CancellationToken ct)
    {
        var l = await GuideHandlers.OwnProfile(profiles, currentUser, r.ProfileId, ct);

        if (l.Error is not null)
            return Result.Failure<GuideAvailabilityReadDto>(l.Error);

        try
        {
            var ids = l.Profile!.Availability.Select(x => x.Id).ToHashSet();
            l.Profile.AddAvailability(r.Dto.Day, r.Dto.StartTime, r.Dto.EndTime);
            await unitOfWork.SaveChangesAsync(ct);
            return GuideAvailabilityReadDto.FromEntity(l.Profile.Availability.First(x => !ids.Contains(x.Id)));
        }
        catch (ArgumentException ex)
        {
            return GuideErrors.Validation<GuideAvailabilityReadDto>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return GuideErrors.Conflict<GuideAvailabilityReadDto>(ex.Message);
        }
    }
}

/// <summary>
/// Handles removing availability periods from guide profiles.
/// </summary>
/// <param name="p">The guide profile repository.</param>
/// <param name="u">Provides information about the current user.</param>
/// <param name="w">Coordinates persistence operations.</param>
public sealed class RemoveGuideAvailabilityHandler(IGuideProfileRepository p, ICurrentUser u, IUnitOfWork w) : MediatR.IRequestHandler<RemoveGuideAvailabilityCommand, Result>
{
    /// <summary>
    /// Removes an availability entry from a guide profile.
    /// </summary>
    /// <param name="r">The removal command.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A successful result or an error if removal fails.</returns>
    public async Task<Result> Handle(RemoveGuideAvailabilityCommand r, CancellationToken ct)
        => await GuideHandlers.Remove(p, u, w, r.ProfileId, r.ItemId, x => x.Availability.FirstOrDefault(i => i.Id == r.ItemId), (x, i) => x.RemoveAvailability(i.Day, i.StartTime), ct);
}

/// <summary>
/// Handles adding time-off entries to guide profiles.
/// </summary>
/// <param name="profiles">The guide profile repository.</param>
/// <param name="currentUser">Provides information about the current user.</param>
/// <param name="unitOfWork">Coordinates persistence operations.</param>
public sealed class AddGuideTimeOffHandler(IGuideProfileRepository profiles, ICurrentUser currentUser, IUnitOfWork unitOfWork) : MediatR.IRequestHandler<AddGuideTimeOffCommand, Result<GuideTimeOffReadDto>>
{
    /// <summary>
    /// Adds a time-off entry to a guide profile owned by the current user.
    /// </summary>
    /// <param name="r">The command containing the time-off data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The added time-off entry, or an error if the operation fails.</returns>
    public async Task<Result<GuideTimeOffReadDto>> Handle(AddGuideTimeOffCommand r, CancellationToken ct)
    {
        var l = await GuideHandlers.OwnProfile(profiles, currentUser, r.ProfileId, ct);

        if (l.Error is not null)
            return Result.Failure<GuideTimeOffReadDto>(l.Error);

        try
        {
            var ids = l.Profile!.TimeOff.Select(x => x.Id).ToHashSet();
            l.Profile.AddTimeOff(r.Dto.Date, r.Dto.Reason);
            await unitOfWork.SaveChangesAsync(ct);
            return GuideTimeOffReadDto.FromEntity(l.Profile.TimeOff.First(x => !ids.Contains(x.Id)));
        }
        catch (ArgumentException ex)
        {
            return GuideErrors.Validation<GuideTimeOffReadDto>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return GuideErrors.Conflict<GuideTimeOffReadDto>(ex.Message);
        }
    }
}

/// <summary>
/// Handles removing time-off entries from guide profiles.
/// </summary>
/// <param name="p">The guide profile repository.</param>
/// <param name="u">Provides information about the current user.</param>
/// <param name="w">Coordinates persistence operations.</param>
public sealed class RemoveGuideTimeOffHandler(IGuideProfileRepository p, ICurrentUser u, IUnitOfWork w) : MediatR.IRequestHandler<RemoveGuideTimeOffCommand, Result>
{
    /// <summary>
    /// Removes a time-off entry from a guide profile.
    /// </summary>
    /// <param name="r">The removal command.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A successful result or an error if removal fails.</returns>
    public async Task<Result> Handle(RemoveGuideTimeOffCommand r, CancellationToken ct)
        => await GuideHandlers.Remove(p, u, w, r.ProfileId, r.ItemId, x => x.TimeOff.FirstOrDefault(i => i.Id == r.ItemId), (x, i) => x.RemoveTimeOff(i.Date), ct);
}

/// <summary>
/// Handles adding pricing options to guide profiles.
/// </summary>
/// <param name="profiles">The guide profile repository.</param>
/// <param name="currentUser">Provides information about the current user.</param>
/// <param name="unitOfWork">Coordinates persistence operations.</param>
public sealed class AddGuidePricingHandler(IGuideProfileRepository profiles, ICurrentUser currentUser, IUnitOfWork unitOfWork) : MediatR.IRequestHandler<AddGuidePricingCommand, Result<GuidePricingReadDto>>
{
    /// <summary>
    /// Adds a pricing option to a guide profile owned by the current user.
    /// </summary>
    /// <param name="r">The command containing the pricing data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The added pricing entry, or an error if the operation fails.</returns>
    public async Task<Result<GuidePricingReadDto>> Handle(AddGuidePricingCommand r, CancellationToken ct)
    {
        var l = await GuideHandlers.OwnProfile(profiles, currentUser, r.ProfileId, ct);

        if (l.Error is not null)
            return Result.Failure<GuidePricingReadDto>(l.Error);

        try
        {
            var ids = l.Profile!.Pricing.Select(x => x.Id).ToHashSet();
            l.Profile.AddPricing(r.Dto.Unit, r.Dto.Amount, r.Dto.Currency);
            await unitOfWork.SaveChangesAsync(ct);
            return GuidePricingReadDto.FromEntity(l.Profile.Pricing.First(x => !ids.Contains(x.Id)));
        }
        catch (ArgumentException ex)
        {
            return GuideErrors.Validation<GuidePricingReadDto>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return GuideErrors.Conflict<GuidePricingReadDto>(ex.Message);
        }
    }
}

/// <summary>
/// Handles removing pricing options from guide profiles.
/// </summary>
/// <param name="p">The guide profile repository.</param>
/// <param name="u">Provides information about the current user.</param>
/// <param name="w">Coordinates persistence operations.</param>
public sealed class RemoveGuidePricingHandler(IGuideProfileRepository p, ICurrentUser u, IUnitOfWork w) : MediatR.IRequestHandler<RemoveGuidePricingCommand, Result>
{
    /// <summary>
    /// Removes a pricing entry from a guide profile.
    /// </summary>
    /// <param name="r">The removal command.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A successful result or an error if removal fails.</returns>
    public async Task<Result> Handle(RemoveGuidePricingCommand r, CancellationToken ct)
        => await GuideHandlers.Remove(p, u, w, r.ProfileId, r.ItemId, x => x.Pricing.FirstOrDefault(i => i.Id == r.ItemId), (x, i) => x.RemovePricing(i.Unit, i.Currency), ct);
}

/// <summary>
/// Provides shared helper methods for guide-related request handlers.
/// </summary>
internal static class GuideHandlers
{
    /// <summary>
    /// Retrieves a guide profile and verifies that it belongs to the current user.
    /// </summary>
    /// <param name="p">The guide profile repository.</param>
    /// <param name="u">Provides information about the current user.</param>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The guide profile if owned by the current user; otherwise, an error.</returns>
    public static async Task<(GuideProfile? Profile, Error? Error)> OwnProfile(IGuideProfileRepository p, ICurrentUser u, Guid id, CancellationToken ct)
    {
        var profile = await p.GetByIdWithDetailsAsync(id, ct);

        if (profile is null)
            return (null, GuideErrors.NotFoundError());

        return profile.UserId != u.UserId
            ? (null, GuideErrors.ForbiddenError())
            : (profile, null);
    }

    /// <summary>
    /// Removes a child entity from a guide profile after verifying profile ownership.
    /// </summary>
    /// <typeparam name="T">The type of child entity being removed.</typeparam>
    /// <param name="p">The guide profile repository.</param>
    /// <param name="u">Provides information about the current user.</param>
    /// <param name="w">Coordinates persistence operations.</param>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="itemId">The unique identifier of the child entity.</param>
    /// <param name="find">A function that locates the child entity in the guide profile.</param>
    /// <param name="remove">An action that removes the child entity through the aggregate root.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A successful result or an error describing why the removal failed.</returns>
    public static async Task<Result> Remove<T>(IGuideProfileRepository p, ICurrentUser u, IUnitOfWork w, Guid id, Guid itemId, Func<GuideProfile, T?> find, Action<GuideProfile, T> remove, CancellationToken ct) where T : class
    {
        var l = await OwnProfile(p, u, id, ct);

        if (l.Error is not null)
            return Result.Failure(l.Error);

        var item = find(l.Profile!);

        if (item is null)
            return GuideErrors.NotFound();

        try
        {
            remove(l.Profile!, item);
            await w.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return GuideErrors.Validation(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return GuideErrors.Conflict(ex.Message);
        }
    }
}

/// <summary>
/// Provides standardized errors for guide-related application operations.
/// </summary>
internal static class GuideErrors
{
    /// <summary>
    /// Creates a not-found error for a requested guide resource.
    /// </summary>
    /// <returns>A not-found error.</returns>
    public static Error NotFoundError()
        => Error.NotFound("Guides.NotFound", "The requested resource was not found.");

    /// <summary>
    /// Creates an authorization error for a user who does not own the requested resource.
    /// </summary>
    /// <returns>An unauthorized error.</returns>
    public static Error ForbiddenError()
        => Error.Unauthorized("Guides.Forbidden", "You are not allowed to access this resource.");

    /// <summary>
    /// Creates a failed result representing a not-found error.
    /// </summary>
    /// <returns>A failed result.</returns>
    public static Result NotFound()
        => Result.Failure(NotFoundError());

    /// <summary>
    /// Creates a failed generic result representing a not-found error.
    /// </summary>
    /// <typeparam name="T">The expected result value type.</typeparam>
    /// <param name="_">The resource name (currently unused).</param>
    /// <returns>A failed generic result.</returns>
    public static Result<T> NotFound<T>(string _)
        => Result.Failure<T>(NotFoundError());

    /// <summary>
    /// Creates a failed result indicating that authentication is required.
    /// </summary>
    /// <returns>A failed result.</returns>
    public static Result Unauthorized()
        => Result.Failure(Error.Unauthorized("Guides.Unauthenticated", "Authentication is required."));

    /// <summary>
    /// Creates a failed generic result indicating that authentication is required.
    /// </summary>
    /// <typeparam name="T">The expected result value type.</typeparam>
    /// <returns>A failed generic result.</returns>
    public static Result<T> Unauthenticated<T>()
        => Result.Failure<T>(Error.Unauthorized("Guides.Unauthenticated", "Authentication is required."));

    /// <summary>
    /// Creates a failed result indicating that the current user is not authorized to access a resource.
    /// </summary>
    /// <returns>A failed result.</returns>
    public static Result Forbidden()
        => Result.Failure(ForbiddenError());

    /// <summary>
    /// Creates a failed generic result indicating that the current user is not authorized to access a resource.
    /// </summary>
    /// <typeparam name="T">The expected result value type.</typeparam>
    /// <returns>A failed generic result.</returns>
    public static Result<T> Forbidden<T>()
        => Result.Failure<T>(ForbiddenError());

    /// <summary>
    /// Creates a failed result representing a validation error.
    /// </summary>
    /// <param name="d">A description of the validation failure.</param>
    /// <returns>A failed result.</returns>
    public static Result Validation(string d)
        => Result.Failure(Error.Validation("Guides.Validation", d));

    /// <summary>
    /// Creates a failed generic result representing a validation error.
    /// </summary>
    /// <typeparam name="T">The expected result value type.</typeparam>
    /// <param name="d">A description of the validation failure.</param>
    /// <returns>A failed generic result.</returns>
    public static Result<T> Validation<T>(string d)
        => Result.Failure<T>(Error.Validation("Guides.Validation", d));

    /// <summary>
    /// Creates a failed result representing a conflict error.
    /// </summary>
    /// <param name="d">A description of the conflict.</param>
    /// <returns>A failed result.</returns>
    public static Result Conflict(string d)
        => Result.Failure(Error.Conflict("Guides.Conflict", d));

    /// <summary>
    /// Creates a failed generic result representing a conflict error.
    /// </summary>
    /// <typeparam name="T">The expected result value type.</typeparam>
    /// <param name="d">A description of the conflict.</param>
    /// <returns>A failed generic result.</returns>
    public static Result<T> Conflict<T>(string d)
        => Result.Failure<T>(Error.Conflict("Guides.Conflict", d));
}