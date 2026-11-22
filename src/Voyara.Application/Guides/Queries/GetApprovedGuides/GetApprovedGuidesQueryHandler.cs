using MediatR;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Queries.GetApprovedGuides;

public sealed class GetApprovedGuidesQueryHandler
    : IRequestHandler<GetApprovedGuidesQuery, Result<PagedResult<GuideListItemDto>>>
{
    private readonly IGuideReadRepository _readRepository;

    public GetApprovedGuidesQueryHandler(IGuideReadRepository readRepository) => _readRepository = readRepository;

    public async Task<Result<PagedResult<GuideListItemDto>>> Handle(
        GetApprovedGuidesQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var criteria = new GuideSearchCriteria(
            request.CityId,
            string.IsNullOrWhiteSpace(request.LanguageCode) ? null : request.LanguageCode.Trim().ToLowerInvariant(),
            request.ExpertiseCategoryId,
            request.MinRating,
            request.MinPrice,
            request.MaxPrice,
            request.PricingUnit,
            string.IsNullOrWhiteSpace(request.Currency) ? null : request.Currency.Trim().ToUpperInvariant());

        return await _readRepository.SearchApprovedAsync(criteria, page, pageSize, cancellationToken);
    }
}
