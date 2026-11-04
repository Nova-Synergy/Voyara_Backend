using MediatR;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Queries.Users.SearchUsers;

public sealed class SearchUsersQueryHandler
    : IRequestHandler<SearchUsersQuery, Result<PagedResult<UserReadDto>>>
{
    private static readonly string[] AllowedRoles =
        [Roles.Tourist, Roles.TourGuide, Roles.Admin];

    private readonly IUserRepository _users;

    public SearchUsersQueryHandler(IUserRepository users) => _users = users;

    public async Task<Result<PagedResult<UserReadDto>>> Handle(
        SearchUsersQuery request,
        CancellationToken cancellationToken)
    {
        string? roleName = null;

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            roleName = AllowedRoles.FirstOrDefault(role =>
                string.Equals(
                    role,
                    request.Role.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (roleName is null)
            {
                return Result.Failure<PagedResult<UserReadDto>>(
                    Error.Validation(
                        "Identity.InvalidRole",
                        $"'{request.Role}' is not a valid role. Allowed roles: Tourist, TourGuide, Admin."));
            }
        }

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, total) = await _users.SearchAsync(
            request.SearchTerm,
            roleName,
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