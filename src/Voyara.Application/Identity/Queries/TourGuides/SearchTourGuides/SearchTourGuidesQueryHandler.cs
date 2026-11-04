using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Queries.TourGuides.SearchTourGuides;

public sealed class SearchTourGuidesQueryHandler
    : IRequestHandler<SearchTourGuidesQuery, Result<PagedResult<TourGuideReadDto>>>
{
    private readonly IGuideProfileRepository _guides;
    private readonly ICurrentUser _currentUser;

    public SearchTourGuidesQueryHandler(
        IGuideProfileRepository guides,
        ICurrentUser currentUser)
    {
        _guides = guides;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<TourGuideReadDto>>> Handle(
        SearchTourGuidesQuery request,
        CancellationToken cancellationToken)
    {
        var isAdmin = _currentUser.IsInRole(Roles.Admin);
        var effectiveStatus = isAdmin
            ? request.Status
            : GuideStatus.Approved;

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, total) = await _guides.SearchAsync(
            request.CityId,
            effectiveStatus,
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<TourGuideReadDto>(
            items.Select(TourGuideReadDto.FromEntity).ToList(),
            total,
            page,
            pageSize);
    }
}