using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Identity;

namespace Voyara.Application.Identity.Queries.Users.SearchUsers;

/// <summary>Searches users by name or e-mail, optionally by role, with paging.</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record SearchUsersQuery(
    string? SearchTerm,
    string? Role,
    int Page,
    int PageSize) : IQuery<PagedResult<UserReadDto>>;
