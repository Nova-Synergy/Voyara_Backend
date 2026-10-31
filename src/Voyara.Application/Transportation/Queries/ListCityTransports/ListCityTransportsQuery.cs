using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Transportation.Queries.ListCityTransports;

/// <summary>Lists the transport options of a city.</summary>
public sealed record ListCityTransportsQuery(int CityId) : IQuery<List<CityTransportSummaryResponse>>;
