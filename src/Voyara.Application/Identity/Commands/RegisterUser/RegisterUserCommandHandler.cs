using MediatR;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Identity;
using Voyara.Application.DTOs;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;
using Voyara.Application.Abstractions;

namespace Voyara.Application.Identity.Commands.RegisterUser;

/// <summary>Creates the account, hashes the password and grants the Tourist role.</summary>
public sealed class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, Result<RegisteredUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<RegisteredUserResponse>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            return Result.Failure<RegisteredUserResponse>(
                Error.Conflict("Auth.EmailAlreadyRegistered", "An account with this e-mail address already exists."));
        }

        var touristRole = await _roleRepository.GetByNameAsync(Roles.Tourist, cancellationToken);
        if (touristRole is null)
        {
            return Result.Failure<RegisteredUserResponse>(
                Error.Failure("Auth.RoleNotConfigured", $"The '{Roles.Tourist}' role is not configured."));
        }

        var user = User.Create(request.FullName, request.Email, _passwordHasher.Hash(request.Password));
        user.AssignRole(touristRole);

        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisteredUserResponse(user.Id, user.FullName, user.Email);
    }
}
