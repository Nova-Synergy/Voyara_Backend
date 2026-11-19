using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Geography.Queries.GetCityById;

public sealed record GetCityByIdQuery(int Id) : IQuery<CityDetailsResponse>;