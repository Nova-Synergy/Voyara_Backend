using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Repositories.Identity;

namespace Voyara.Infrastructure.Persistence.Repositories.Identity
{

    /// <summary>
    /// EF Core implementation of <see cref="IUserRepository"/>.
    /// </summary>
    public sealed class UserRepository : IUserRepository
    {
        private readonly VoyaraDbContext _context;
    }
}