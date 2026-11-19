using MediatR;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Geography;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Geography.Queries.GetCountryById;

public sealed class GetCountryByIdQueryHandler
    : IRequestHandler<GetCountryByIdQuery, Result<CountryDetailsResponse>>
{
    private readonly ICountryRepository _countryRepository;

    public GetCountryByIdQueryHandler(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<Result<CountryDetailsResponse>> Handle(
        GetCountryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var country = await _countryRepository.GetByIdWithCitiesAsync(
            request.Id, cancellationToken);

        if (country is null)
        {
            return Result.Failure<CountryDetailsResponse>(
                Error.NotFound(
                    "Country.NotFound",
                    $"Country with ID '{request.Id}' was not found."));
        }

        var response = new CountryDetailsResponse(
            country.Id,
            country.IsCode,
            country.DefaultCurrency,
            country.TimeZone,
            country.IsActive,
            country.Cities.Count);

        return Result.Success(response);
    }
}