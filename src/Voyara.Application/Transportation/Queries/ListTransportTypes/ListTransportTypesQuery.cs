using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Transportation.Queries.ListTransportTypes;

/// <summary>Lists all transport types.</summary>
public sealed record ListTransportTypesQuery : IQuery<List<TransportTypeResponse>>;
