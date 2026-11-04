using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Identity;

namespace Voyara.Application.Identity.Queries.Tourists.SearchTourists;

/// <summary>Searches tourist profiles with paging. Admin only.</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record SearchTouristsQuery(
    string? SearchTerm,
    int Page,
    int PageSize) : IQuery<PagedResult<TouristReadDto>>;
