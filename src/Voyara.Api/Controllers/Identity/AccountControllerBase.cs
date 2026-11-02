using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Domain.Entities.Identity;
using Voyara.Infrastructure.Persistence;
using UserEntity = Voyara.Domain.Entities.Identity.User;

namespace Voyara.Api.Controllers.Identity;

/// <summary>Shared account logic for the user, admin, tourist and tour guide controllers.</summary>
public abstract class AccountControllerBase : DomainControllerBase
{
    private static readonly string[] AllowedRoles = [Roles.Tourist, Roles.TourGuide, Roles.Admin];

    protected AccountControllerBase(VoyaraDbContext context, ICurrentUser currentUser)
    {
        Context = context;
        CurrentUser = currentUser;
    }

    protected VoyaraDbContext Context { get; }

    // ControllerBase already has a "User" (claims) property, hence the "CurrentUser" name.
    protected ICurrentUser CurrentUser { get; }

    protected bool IsAdmin => CurrentUser.IsInRole(Roles.Admin);

    /// <summary>True for an administrator or for the account owner.</summary>
    protected bool IsAdminOrSelf(Guid userId) => IsAdmin || CurrentUser.UserId == userId;

    /// <summary>Checks e-mail uniqueness, including soft-deleted accounts (their e-mail stays reserved).</summary>
    protected Task<bool> EmailTakenAsync(string email, CancellationToken cancellationToken)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return Context.Users.IgnoreQueryFilters().AsNoTracking().AnyAsync(u => u.Email == normalized, cancellationToken);
    }

    /// <summary>Loads a tracked user with roles and refresh tokens, optionally restricted to a role.</summary>
    protected Task<UserEntity?> FindUserAsync(Guid id, string? requiredRole, CancellationToken cancellationToken)
    {
        IQueryable<UserEntity> query = Context.Users.Include(u => u.Roles).Include(u => u.RefreshTokens);
        if (requiredRole is not null)
        {
            query = query.Where(u => u.Roles.Any(r => r.Name == requiredRole));
        }

        return query.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <summary>Normalizes role names to their canonical spelling; returns false for an unknown role.</summary>
    protected static bool TryNormalizeRoles(IEnumerable<string> names, out List<string> roles, out string? invalid)
    {
        roles = [];
        invalid = null;

        foreach (var name in names)
        {
            var canonical = AllowedRoles.FirstOrDefault(r => string.Equals(r, name?.Trim(), StringComparison.OrdinalIgnoreCase));
            if (canonical is null)
            {
                invalid = name;
                return false;
            }

            if (!roles.Contains(canonical))
            {
                roles.Add(canonical);
            }
        }

        return roles.Count > 0;
    }

    /// <summary>Gets a role by name, or creates it: roles are not seeded, so the first use creates the row.</summary>
    protected async Task<Role> GetOrCreateRoleAsync(string name, CancellationToken cancellationToken)
    {
        var lowered = name.ToLowerInvariant();
        var role = await Context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == lowered, cancellationToken);
        if (role is not null)
        {
            return role;
        }

        role = Role.Create(name);
        Context.Roles.Add(role);
        return role;
    }

    /// <summary>Makes the user's roles exactly the given set.</summary>
    protected async Task ReplaceRolesAsync(UserEntity user, IReadOnlyCollection<string> target, CancellationToken cancellationToken)
    {
        foreach (var role in user.Roles.Where(r => !target.Contains(r.Name, StringComparer.OrdinalIgnoreCase)).ToList())
        {
            user.RemoveRole(role);
        }

        foreach (var name in target.Where(n => !user.HasRole(n)))
        {
            user.AssignRole(await GetOrCreateRoleAsync(name, cancellationToken));
        }
    }

    /// <summary>Activates or deactivates the user when the requested state differs from the current one.</summary>
    protected ActionResult? ApplyActivation(UserEntity user, bool? isActive)
    {
        if (isActive is null || isActive == user.IsActive)
        {
            return null;
        }

        if (!isActive.Value && user.Id == CurrentUser.UserId)
        {
            return Conflict("You cannot deactivate your own account.");
        }

        return ApplyDomainChange(() =>
        {
            if (isActive.Value)
            {
                user.Activate();
            }
            else
            {
                user.Deactivate(DateTimeOffset.UtcNow);
            }
        });
    }

    /// <summary>
    /// Soft-deletes the account: it is deactivated, its refresh tokens are revoked and it is flagged as deleted
    /// (the global filter then hides it). The row, its roles and its audit trail are kept.
    /// </summary>
    protected static void SoftDelete(UserEntity user)
    {
        var now = DateTimeOffset.UtcNow;
        if (user.IsActive)
        {
            user.Deactivate(now);
        }
        else
        {
            user.RevokeAllRefreshTokens(now);
        }

        user.IsDeleted = true;
    }
}
