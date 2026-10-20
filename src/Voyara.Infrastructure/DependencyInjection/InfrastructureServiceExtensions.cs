using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Catalog;
using Voyara.Domain.Repositories.Culture_Rules;
using Voyara.Infrastructure.Ai;
using Voyara.Infrastructure.Caching;
using Voyara.Infrastructure.Currency;
using Voyara.Infrastructure.FileStorage;
using Voyara.Infrastructure.Identity;
using Voyara.Infrastructure.Notifications;
using Voyara.Infrastructure.Persistence;
using Voyara.Infrastructure.Persistence.Interceptors;
using Voyara.Infrastructure.Persistence.Outbox;
using Voyara.Infrastructure.Persistence.Repositories.Catalog;
using Voyara.Infrastructure.Persistence.Repositories.Culture_Rules;
using Voyara.Infrastructure.Recommendations;

namespace Voyara.Infrastructure.DependencyInjection;

/// <summary>Composition root for the Infrastructure layer.</summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>Registers persistence, caching, adapters, decorators and outbox services.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>().Bind(configuration.GetSection(DatabaseOptions.SectionName));
        services.AddOptions<RedisOptions>().Bind(configuration.GetSection(RedisOptions.SectionName));
        services.AddOptions<FileStorageOptions>().Bind(configuration.GetSection(FileStorageOptions.SectionName));
        services.AddOptions<AiOptions>().Bind(configuration.GetSection(AiOptions.SectionName));

        services.AddScoped<SaveChangesInterceptor, AuditInterceptor>();
        services.AddScoped<SaveChangesInterceptor, OutboxInterceptor>();

        services.AddDbContext<VoyaraDbContext>((sp, options) =>
        {
            var databaseOptions = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<DatabaseOptions>>().Value;
            options.UseNpgsql(databaseOptions.ConnectionString, npgsql => npgsql.UseNetTopologySuite())
                .AddInterceptors(sp.GetServices<SaveChangesInterceptor>().ToArray());
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDestinationRepository, DestinationRepository>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddSingleton<IDateTime, SystemDateTime>();

        services.AddSingleton(sp =>
        {
            var redisOptions = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RedisOptions>>().Value;
            return new Lazy<IConnectionMultiplexer>(
                () => ConnectionMultiplexer.Connect(redisOptions.ConnectionString),
                LazyThreadSafetyMode.ExecutionAndPublication);
        });
        services.AddScoped<ICacheService>(sp => new RedisCacheService(
            sp.GetRequiredService<Lazy<IConnectionMultiplexer>>().Value));

        services.AddSingleton<IFileStorage, LocalFileStorage>();

        services.AddSingleton<IAiProvider>(sp => new UsageLoggingAiProvider(
            new RateLimitedAiProvider(
                new StubAiProvider(),
                sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AiOptions>>().Value),
            sp.GetRequiredService<ILogger<UsageLoggingAiProvider>>()));

        services.AddSingleton<IExchangeRateProvider>(sp => new CachedExchangeRateProvider(
            new StubExchangeRateProvider(),
            sp.GetRequiredService<ILogger<CachedExchangeRateProvider>>()));

        services.AddScoped<INotificationChannel, InAppNotificationChannel>();
        services.AddScoped<INotificationChannel, EmailNotificationChannel>();

        services.AddSingleton<IRecommendationStrategy, PopularityRecommendationStrategy>();

        services.AddScoped<IOutboxProcessor, OutboxProcessor>();
        services.AddScoped<ILocalRuleRepository, LocalRuleRepository>();

        return services;
    }
}
