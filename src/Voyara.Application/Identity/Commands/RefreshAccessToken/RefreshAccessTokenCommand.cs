using MediatR;
using Voyara.Application.DTOs;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.RefreshAccessToken;

/// <summary>
/// Exchanges a refresh token for a new token pair (rotation). Deliberately not an
/// <c>IBaseCommand</c>: when reuse of a revoked token is detected the handler fails but must still
/// persist the revocation of every session, which the transaction behavior would roll back.
/// </summary>
public sealed record RefreshAccessTokenCommand(string RefreshToken) : IRequest<Result<AuthTokensResponse>>;
