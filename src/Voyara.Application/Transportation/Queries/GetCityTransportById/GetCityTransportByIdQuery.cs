using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Transportation.Queries.GetCityTransportById;

/// <summary>Gets a city transport with its routes, stations and linked destinations.</summary>
public sealed record GetCityTransportByIdQuery(Guid CityTransportId) : IQuery<CityTransportResponse>;
