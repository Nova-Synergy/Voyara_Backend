using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Identity.Commands.RegisterUser;

/// <summary>Registers a new tourist account. Anonymous callers are allowed.</summary>
public sealed record RegisterUserCommand(
    string FullName,
    string Email,
    string Password) : ICommand<RegisteredUserResponse>;
