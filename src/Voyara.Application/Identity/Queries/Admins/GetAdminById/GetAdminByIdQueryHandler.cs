using MediatR;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Queries.Admins.GetAdminById;

public sealed class GetAdminByIdQueryHandler
    : IRequestHandler<GetAdminByIdQuery, Result<UserReadDto>>
{
    private readonly IUserRepository _users;

    public GetAdminByIdQueryHandler(IUserRepository users) => _users = users;

    public async Task<Result<UserReadDto>> Handle(
        GetAdminByIdQuery request,
        CancellationToken cancellationToken)
    {
        var admin = await _users.GetByIdReadOnlyAsync(
            request.Id,
            Roles.Admin,
            cancellationToken);

        return admin is null
            ? Result.Failure<UserReadDto>(
                Error.NotFound(
                    "Identity.NotFound",
                    "Administrator was not found."))
            : UserReadDto.FromEntity(admin);
    }
}