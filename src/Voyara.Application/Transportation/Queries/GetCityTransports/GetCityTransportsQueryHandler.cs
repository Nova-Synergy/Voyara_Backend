using MediatR;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Transportation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Transportation.Queries.GetCityTransports;

/// <summary>Handles <see cref="GetCityTransportsQuery"/>.</summary>
public sealed class GetCityTransportsQueryHandler
    : IRequestHandler<GetCityTransportsQuery, Result<PagedResult<CityTransportListItemDto>>>
{
    private readonly ICityTransportReadRepository _readRepository;

    /// <summary>Creates the handler.</summary>
    public GetCityTransportsQueryHandler(ICityTransportReadRepository readRepository)
        => _readRepository = readRepository;

    /// <inheritdoc />
    public async Task<Result<PagedResult<CityTransportListItemDto>>> Handle(
        GetCityTransportsQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var criteria = new CityTransportSearchCriteria(
            request.CityId,
            request.TransportTypeId,
            request.DestinationId,
            request.MinCost,
            request.MaxCost,
            string.IsNullOrWhiteSpace(request.Currency) ? null : request.Currency.Trim().ToUpperInvariant());

        return await _readRepository.SearchAsync(criteria, page, pageSize, cancellationToken);
    }
}
