using MediatR;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Geography;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Geography.Queries.GetCityById;

public sealed class GetCityByIdQueryHandler
    : IRequestHandler<GetCityByIdQuery, Result<CityDetailsResponse>>
{
    private readonly ICityRepository _cityRepository;

    public GetCityByIdQueryHandler(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<Result<CityDetailsResponse>> Handle(
        GetCityByIdQuery request,
        CancellationToken cancellationToken)
    {
        var city = await _cityRepository.GetByIdWithCountryAsync(
            request.Id, cancellationToken);

        if (city is null)
        {
            return Result.Failure<CityDetailsResponse>(
                Error.NotFound(
                    "City.NotFound",
                    $"City with ID '{request.Id}' was not found."));
        }

        var response = new CityDetailsResponse(
            city.Id,
            city.CountryId,
            city.Country.IsCode,
            city.Latitude,
            city.Longitude,
            city.IsActive);

        return Result.Success(response);
    }
}