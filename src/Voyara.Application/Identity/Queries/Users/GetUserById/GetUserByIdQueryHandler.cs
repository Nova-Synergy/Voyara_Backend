using MediatR;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Queries.Users.GetUserById;

public sealed class GetUserByIdQueryHandler
    : IRequestHandler<GetUserByIdQuery, Result<UserReadDto>>
{
    private readonly IUserRepository _users;

    public GetUserByIdQueryHandler(IUserRepository users) => _users = users;

    public async Task<Result<UserReadDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdReadOnlyAsync(
            request.Id,
            null,
            cancellationToken);

        return user is null
            ? Result.Failure<UserReadDto>(
                Error.NotFound(
                    "Identity.NotFound",
                    "User was not found."))
            : UserReadDto.FromEntity(user);
    }
}