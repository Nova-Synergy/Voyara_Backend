using MediatR;
using Voyara.Application.DTOs.Transportation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Transportation.Queries.GetCityTransportById;

/// <summary>Handles <see cref="GetCityTransportByIdQuery"/>.</summary>
public sealed class GetCityTransportByIdQueryHandler
    : IRequestHandler<GetCityTransportByIdQuery, Result<CityTransportDetailsDto>>
{
    private readonly ICityTransportReadRepository _readRepository;

    /// <summary>Creates the handler.</summary>
    public GetCityTransportByIdQueryHandler(ICityTransportReadRepository readRepository)
        => _readRepository = readRepository;

    /// <inheritdoc />
    public async Task<Result<CityTransportDetailsDto>> Handle(
        GetCityTransportByIdQuery request,
        CancellationToken cancellationToken)
    {
        var details = await _readRepository.GetDetailsAsync(request.Id, cancellationToken);

        return details is null
            ? Result.Failure<CityTransportDetailsDto>(TransportationErrors.NotFound)
            : details;
    }
}
