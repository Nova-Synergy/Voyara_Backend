using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Entities.Community;
using Voyara.Infrastructure.Persistence.Outbox;

namespace Voyara.Infrastructure.Persistence;

/// <summary>EF Core context for the platform; module configurations are applied from this assembly.</summary>
public sealed class VoyaraDbContext : DbContext
{
    /// <summary>Creates the context.</summary>
    public VoyaraDbContext(DbContextOptions<VoyaraDbContext> options)
        : base(options)
    {
    }

    /// <summary>Catalog destinations (reference aggregate).</summary>
    public DbSet<Destination> Destinations => Set<Destination>();

    /// <summary>Tourist reviews.</summary>
    public DbSet<Review> Reviews => Set<Review>();


    /// <summary>Photos attached to reviews.</summary>
    public DbSet<ReviewPhoto> ReviewPhotos => Set<ReviewPhoto>();

    /// <summary>Saved user destinations.</summary>
    public DbSet<Favorite> Favorites => Set<Favorite>();

    /// <summary>Proposals for new tourism places.</summary>
    public DbSet<PlaceSuggestion> PlaceSuggestions
        => Set<PlaceSuggestion>();


    /// <summary>Photos attached to place suggestions.</summary>
    public DbSet<SuggestionPhoto> SuggestionPhotos
        => Set<SuggestionPhoto>();

    /// <summary>Transactional outbox messages.</summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VoyaraDbContext).Assembly);
    }
}
