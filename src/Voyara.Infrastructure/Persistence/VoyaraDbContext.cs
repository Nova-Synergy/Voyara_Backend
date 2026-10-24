using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Entities.Identity;
using Voyara.Infrastructure.Persistence.Outbox;
using Voyara.Domain.Entities.Transportation;
namespace Voyara.Infrastructure.Persistence;

/// <summary>EF Core context for the platform; module configurations are applied from this assembly.</summary>
public sealed class VoyaraDbContext : DbContext
{
    /// <summary>Creates the context.</summary>
    public VoyaraDbContext(DbContextOptions<VoyaraDbContext> options)
        : base(options)
    {
    }

    /// <summary>User accounts (aggregate root: roles, refresh tokens).</summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>Authorization roles.</summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>Tourist interests.</summary>
    public DbSet<Interest> Interests => Set<Interest>();

    /// <summary>Tourist profiles (aggregate root: interests, visit history).</summary>
    public DbSet<TouristProfile> TouristProfiles => Set<TouristProfile>();

    /// <summary>Catalog destinations (reference aggregate).</summary>
    public DbSet<Destination> Destinations => Set<Destination>();

    /// <summary>Transactional outbox messages.</summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <summary>Reference data for transport types.</summary>
    public DbSet<TransportType> TransportTypes => Set<TransportType>();

    /// <summary>City transport aggregates.</summary>
    public DbSet<CityTransport> CityTransports => Set<CityTransport>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VoyaraDbContext).Assembly);
    }
}
