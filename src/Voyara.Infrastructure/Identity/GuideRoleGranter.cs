using Microsoft.EntityFrameworkCore;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Guides;
using Voyara.Domain.Entities.Identity;
using Voyara.Infrastructure.Persistence;
using Voyara.SharedKernel.Results;

namespace Voyara.Infrastructure.Identity 
{

    /// <summary>
    /// Assigns the TourGuide role through the scoped <see cref="VoyaraDbContext"/>, the same context the
    /// approval uses. It never saves: the handler's single SaveChanges inside the command transaction
    /// commits the approval, the role link and the audit entry together.
    /// </summary>
    public sealed class GuideRoleGranter : IGuideRoleGranter
    {
        private readonly VoyaraDbContext _context;

        public GuideRoleGranter(VoyaraDbContext context) => _context = context;

        public async Task<Result> GrantTourGuideRoleAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Set<User>()
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null)
            {
                return Result.Failure(Error.NotFound("Guides.UserNotFound", "The guide's user account was not found."));
            }

            var role = await _context.Set<Role>()
                .FirstOrDefaultAsync(r => r.Name == Roles.TourGuide, cancellationToken);

            if (role is null)
            {
                // The roles table is not seeded yet, so the role is created inside the same transaction.
                role = Role.Create(Roles.TourGuide);
                _context.Set<Role>().Add(role);
            }

            user.AssignRole(role);
            return Result.Success();
        }
    }
}
