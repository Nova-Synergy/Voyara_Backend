using MediatR;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Geography;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Geography.Queries.GetCountries;

public sealed class GetCountriesQueryHandler
    : IRequestHandler<GetCountriesQuery, Result<IReadOnlyList<CountryResponse>>>
{
    private readonly ICountryRepository _countryRepository;

    public GetCountriesQueryHandler(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<Result<IReadOnlyList<CountryResponse>>> Handle(
        GetCountriesQuery request,
        CancellationToken cancellationToken)
    {
        var countries = await _countryRepository.GetAllAsync(
            request.IsActive,
            request.Search,
            cancellationToken);

        var response = countries
            .Select(c => new CountryResponse(
                c.Id,
                c.IsCode,
                c.DefaultCurrency,
                c.TimeZone,
                c.IsActive))
            .ToList();

        return Result.Success<IReadOnlyList<CountryResponse>>(response);
    }
}