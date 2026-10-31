using MediatR;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Transportation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Transportation.Queries.ListCityTransports;

/// <summary>Handles city transport lists.</summary>
public sealed class ListCityTransportsQueryHandler
    : IRequestHandler<ListCityTransportsQuery, Result<List<CityTransportSummaryResponse>>>
{
    private readonly ICityTransportRepository _cityTransportRepository;

    /// <summary>Creates the handler.</summary>
    public ListCityTransportsQueryHandler(ICityTransportRepository cityTransportRepository)
        => _cityTransportRepository = cityTransportRepository;

    /// <inheritdoc />
    public async Task<Result<List<CityTransportSummaryResponse>>> Handle(
        ListCityTransportsQuery request,
        CancellationToken cancellationToken)
    {
        var cityTransports = await _cityTransportRepository.ListByCityAsync(request.CityId, cancellationToken);

        var items = cityTransports
            .Select(cityTransport => cityTransport.ToSummary())
            .ToList();

        return items;
    }
}
