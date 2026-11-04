using MediatR;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Queries.Admins.SearchAdmins;

public sealed class SearchAdminsQueryHandler
    : IRequestHandler<SearchAdminsQuery, Result<PagedResult<UserReadDto>>>
{
    private readonly IUserRepository _users;

    public SearchAdminsQueryHandler(IUserRepository users) => _users = users;

    public async Task<Result<PagedResult<UserReadDto>>> Handle(
        SearchAdminsQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, total) = await _users.SearchAsync(
            request.SearchTerm,
            Roles.Admin,
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<UserReadDto>(
            items.Select(UserReadDto.FromEntity).ToList(),
            total,
            page,
            pageSize);
    }
}