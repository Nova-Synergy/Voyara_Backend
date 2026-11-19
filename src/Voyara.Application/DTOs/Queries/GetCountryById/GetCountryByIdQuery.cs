using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Geography.Queries.GetCountryById;

public sealed record GetCountryByIdQuery(int Id) : IQuery<CountryDetailsResponse>;