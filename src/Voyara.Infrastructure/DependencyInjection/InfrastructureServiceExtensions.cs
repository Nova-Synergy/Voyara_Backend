
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Catalog;
using Voyara.Domain.Repositories.Transportation;
using Voyara.Domain.Repositories.LocalInfo;

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
using Voyara.Infrastructure.Persistence.Repositories.Transportation;
using Voyara.Infrastructure.Persistence.Repositories.LocalInfo;
using Voyara.Infrastructure.Recommendations;

using CultureRulesContract =
    Voyara.Domain.Repositories.Culture_Rules.ILocalRuleRepository;

using CultureRulesRepository =
    Voyara.Infrastructure.Persistence.Repositories.Culture_Rules.LocalRuleRepository;

using LocalInfoRulesContract =
    Voyara.Domain.Repositories.LocalInfo.ILocalRuleRepository;

using LocalInfoRulesRepository =
    Voyara.Infrastructure.Persistence.Repositories.LocalInfo.LocalRuleRepository;

namespace Voyara.Infrastructure.DependencyInjection;

public static class Infr9yMnTm4NSzvG9rrwjM2ec8xZgh1cafXH8
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddOptions<RedisOptions>()
            .Bind(configuration.GetSection(RedisOptions.SectionName));

        services.AddOptions<FileStorageOptions>()
            .Bind(configuration.GetSection(FileStorageOptions.SectionName));

        services.AddOptions<AiOptions>()
            .Bind(configuration.GetSection(AiOptions.SectionName));

        services.AddScoped<SaveChangesInterceptor, AuditInterceptor>();
        services.AddScoped<SaveChangesInterceptor, OutboxInterceptor>();

        services.AddDbContext<VoyaraDbContext>((sp, options) =>
        {
            var databaseOptions = sp
                .GetRequiredService<
                    Microsoft.Extensions.Options.IOptions<DatabaseOptions>>()
                .Value;

            options.UseNpgsql(
                    databaseOptions.ConnectionString,
                    npgsql => npgsql.UseNetTopologySuite())
                .AddInterceptors(
                    sp.GetServices<SaveChangesInterceptor>().ToArray());
        });

        // Unit Of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Catalog
        services.AddScoped<IDestinationRepository, DestinationRepository>();

        // Identity
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        // DateTime
        services.AddSingleton<IDateTime, SystemDateTime>();

        // Redis Cache
        services.AddSingleton(sp =>
        {
            var redisOptions = sp
                .GetRequiredService<
                    Microsoft.Extensions.Options.IOptions<RedisOptions>>()
                .Value;

            return new Lazy<IConnectionMultiplexer>(
                () => ConnectionMultiplexer.Connect(
                    redisOptions.ConnectionString),
                LazyThreadSafetyMode.ExecutionAndPublication);
        });

        services.AddScoped<ICacheService>(sp =>
            new RedisCacheService(
                sp.GetRequiredService<
                    Lazy<IConnectionMultiplexer>>().Value));

        // File Storage
        services.AddSingleton<IFileStorage, LocalFileStorage>();

        // AI
        services.AddSingleton<IAiProvider>(sp =>
            new UsageLoggingAiProvider(
                new RateLimitedAiProvider(
                    new StubAiProvider(),
                    sp.GetRequiredService<
                        Microsoft.Extensions.Options.IOptions<AiOptions>>().Value),
                sp.GetRequiredService<
                    ILogger<UsageLoggingAiProvider>>()));

        // Currency
        services.AddSingleton<IExchangeRateProvider>(sp =>
            new CachedExchangeRateProvider(
                new StubExchangeRateProvider(),
                sp.GetRequiredService<
                    ILogger<CachedExchangeRateProvider>>()));

        // Notifications
        services.AddScoped<INotificationChannel, InAppNotificationChannel>();
        services.AddScoped<INotificationChannel, EmailNotificationChannel>();

        // Recommendations
        services.AddSingleton<
            IRecommendationStrategy,
            Popular9yMnTm4NSzvG9rrwjM2ec8xZgh1cafXH8>();

        // Outbox
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();

        // Culture & Rules
        services.AddScoped<
            CultureRulesContract,
            CultureRulesRepository>();

        // LocalInfo
        services.AddScoped<
            IAwarenessArticleRepository,
            AwarenessArticleRepository>();

        services.AddScoped<
            ISafetyGuideRepository,
            SafetyGuideRepository>();

        services.AddScoped<
            IEmbassyContactRepository,
            EmbassyContactRepository>();

        services.AddScoped<
            LocalInfoRulesContract,
            LocalInfoRulesRepository>();

        // Transportation
        services.AddScoped<ICityTransportRepository, CityTransportRepository>();
        services.AddScoped<ITransportTypeRepository, TransportTypeRepository>();

        return services;
    }
}
