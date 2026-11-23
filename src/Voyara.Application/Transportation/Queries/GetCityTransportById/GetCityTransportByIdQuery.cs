using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Transportation;

namespace Voyara.Application.Transportation.Queries.GetCityTransportById;

/// <summary>Gets the full details of one city transport.</summary>
public sealed record GetCityTransportByIdQuery(Guid Id) : IQuery<CityTransportDetailsDto>;
