using MediatR;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Transportation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Transportation.Queries.GetCityTransportById;

/// <summary>Handles single city transport lookups.</summary>
public sealed class GetCityTransportByIdQueryHandler
    : IRequestHandler<GetCityTransportByIdQuery, Result<CityTransportResponse>>
{
    private readonly ICityTransportRepository _cityTransportRepository;

    /// <summary>Creates the handler.</summary>
    public GetCityTransportByIdQueryHandler(ICityTransportRepository cityTransportRepository)
        => _cityTransportRepository = cityTransportRepository;

    /// <inheritdoc />
    public async Task<Result<CityTransportResponse>> Handle(
        GetCityTransportByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cityTransport = await _cityTransportRepository.GetByIdAsync(request.CityTransportId, cancellationToken);
        if (cityTransport is null)
        {
            return Result.Failure<CityTransportResponse>(
                TransportationApplicationErrors.CityTransportNotFound(request.CityTransportId));
        }

        return cityTransport.ToResponse();
    }
}
