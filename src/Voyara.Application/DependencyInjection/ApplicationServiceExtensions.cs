using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Voyara.Application.Behaviors;

namespace Voyara.Application.DependencyInjection;

/// <summary>Composition root for the Application layer.</summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Registers MediatR with the pipeline behaviors, FluentValidation validators and Mapster
    /// mapping configurations from this assembly.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(ApplicationServiceExtensions).Assembly);
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            configuration.AddOpenBehavior(typeof(CachingBehavior<,>));
            configuration.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(ApplicationServiceExtensions).Assembly);

        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(typeof(ApplicationServiceExtensions).Assembly);
        services.AddSingleton(typeAdapterConfig);
        services.AddMapster();

        return services;
    }
}
