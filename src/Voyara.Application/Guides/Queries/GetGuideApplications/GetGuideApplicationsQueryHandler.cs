using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guide;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Queries.GetGuideApplications;

public sealed class GetGuideApplicationsQueryHandler
    : IRequestHandler<GetGuideApplicationsQuery, Result<PagedResult<GuideApplicationListItemDto>>>
{
    private readonly IGuideReadRepository _readRepository;

    public GetGuideApplicationsQueryHandler(IGuideReadRepository readRepository) => _readRepository = readRepository;

    public async Task<Result<PagedResult<GuideApplicationListItemDto>>> Handle(
        GetGuideApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        return await _readRepository.ListApplicationsAsync(request.Status, page, pageSize, cancellationToken);
    }
}
