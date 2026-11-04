using MediatR;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Queries.Tourists.SearchTourists;

public sealed class SearchTouristsQueryHandler
    : IRequestHandler<SearchTouristsQuery, Result<PagedResult<TouristReadDto>>>
{
    private readonly ITouristProfileRepository _tourists;

    public SearchTouristsQueryHandler(ITouristProfileRepository tourists)
        => _tourists = tourists;

    public async Task<Result<PagedResult<TouristReadDto>>> Handle(
        SearchTouristsQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, total) = await _tourists.SearchAsync(
            request.SearchTerm,
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<TouristReadDto>(
            items.Select(TouristReadDto.FromEntity).ToList(),
            total,
            page,
            pageSize);
    }
}