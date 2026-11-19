using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Geography.Queries.GetCountries;

public sealed record GetCountriesQuery(
    bool? IsActive = null,
    string? Search = null) : IQuery<IReadOnlyList<CountryResponse>>;