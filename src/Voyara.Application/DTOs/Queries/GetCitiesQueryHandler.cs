using MediatR;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Geography;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Geography.Queries.GetCities;

public sealed class GetCitiesQueryHandler
    : IRequestHandler<GetCitiesQuery, Result<IReadOnlyList<CityResponse>>>
{
    private readonly ICityRepository _cityRepository;

    public GetCitiesQueryHandler(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<Result<IReadOnlyList<CityResponse>>> Handle(
        GetCitiesQuery request,
        CancellationToken cancellationToken)
    {
        var cities = await _cityRepository.GetAllAsync(
            request.CountryId,
            request.IsActive,
            cancellationToken);

        var response = cities
            .Select(c => new CityResponse(
                c.Id,
                c.CountryId,
                c.Latitude,
                c.Longitude,
                c.IsActive))
            .ToList();

        return Result.Success<IReadOnlyList<CityResponse>>(response);
    }
}