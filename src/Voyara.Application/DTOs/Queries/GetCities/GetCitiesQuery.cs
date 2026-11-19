using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Geography.Queries.GetCities;

public sealed record GetCitiesQuery(
    int? CountryId = null,
    bool? IsActive = null) : IQuery<IReadOnlyList<CityResponse>>;