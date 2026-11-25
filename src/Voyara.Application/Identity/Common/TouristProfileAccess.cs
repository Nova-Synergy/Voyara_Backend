using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Common;

/// <summary>Errors and lookup shared by the tourist profile use cases.</summary>
internal static class TouristProfileAccess
{
    /// <summary>The caller has no tourist profile.</summary>
    public static readonly Error NotFound = Error.NotFound(
        "TouristProfile.NotFound", "The tourist profile of the current user was not found.");

    /// <summary>The caller identity could not be resolved.</summary>
    public static readonly Error Unauthenticated = Error.Unauthorized(
        "Authorization.Anonymous", "Authentication is required.");

    /// <summary>The caller's account is deactivated.</summary>
    public static readonly Error AccountDeactivated = Error.Unauthorized(
        "Auth.AccountDeactivated", "This account is deactivated.");

    /// <summary>
    /// Loads the profile of the current user, applying the same checks as the current-user query.
    /// Failures are returned as results: unauthorized when the identity is unknown or the account
    /// is deactivated, not found when the user has no profile.
    /// </summary>
    /// <param name="currentUser">The current caller.</param>
    /// <param name="repository">The profile repository.</param>
    /// <param name="readOnly">True to load without change tracking.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static async Task<Result<TouristProfile>> LoadCurrentAsync(
        ICurrentUser currentUser,
        ITouristProfileRepository repository,
        bool readOnly,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return Result.Failure<TouristProfile>(Unauthenticated);
        }

        var profile = readOnly
            ? await repository.GetReadOnlyByUserIdAsync(userId, cancellationToken)
            : await repository.GetByUserIdAsync(userId, cancellationToken);

        if (profile is null)
        {
            return Result.Failure<TouristProfile>(NotFound);
        }

        if (!profile.User.IsActive)
        {
            return Result.Failure<TouristProfile>(AccountDeactivated);
        }

        return Result.Success(profile);
    }
}
