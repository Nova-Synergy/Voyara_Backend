using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Entities.Culture_Rules;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Entities.Transportation;
using Voyara.Infrastructure.Persistence.Outbox;
namespace Voyara.Infrastructure.Persistence;

using Voyara.Domain.Entities.Geography;
using Voyara.Domain.Entities.Localization;

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

    public DbSet<Language> Languages => Set<Language>();
    /// <summary>Authorization roles.</summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>Tourist interests.</summary>
    public DbSet<Interest> Interests => Set<Interest>();

    /// <summary>Tourist profiles (aggregate root: interests, visit history).</summary>
    public DbSet<TouristProfile> TouristProfiles => Set<TouristProfile>();

    /// <summary>Catalog destinations (reference aggregate).</summary>
    public DbSet<Country> Countries => Set<Country>();

    public DbSet<City> Cities => Set<City>();

    /// <summary>Catalog destinations (reference aggregate).</summary>
    public DbSet<Destination> Destinations => Set<Destination>();

    public DbSet<Category> Categories => Set<Category>();
    /// <summary>Transactional outbox messages.</summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <summary>Reference data for transport types.</summary>
    public DbSet<TransportType> TransportTypes => Set<TransportType>();

    /// <summary>City transport aggregates.</summary>
    public DbSet<CityTransport> CityTransports => Set<CityTransport>();

    /// <summary>Cultural, legal and behavioral rule aggregates.</summary>
    public DbSet<LocalRule> LocalRules => Set<LocalRule>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VoyaraDbContext).Assembly);
    }
}
