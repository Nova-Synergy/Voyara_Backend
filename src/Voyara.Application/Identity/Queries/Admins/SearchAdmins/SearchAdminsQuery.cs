using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Identity;

namespace Voyara.Application.Identity.Queries.Admins.SearchAdmins;

/// <summary>Searches administrator accounts with paging.</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record SearchAdminsQuery(
    string? SearchTerm,
    int Page,
    int PageSize) : IQuery<PagedResult<UserReadDto>>;
