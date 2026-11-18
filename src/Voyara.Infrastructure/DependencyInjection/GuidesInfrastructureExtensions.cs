using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voyara.Application.Guides;
using Voyara.Domain.Repositories.Guides;
using Voyara.Infrastructure.FileStorage;
using Voyara.Infrastructure.Identity;
using Voyara.Infrastructure.Persistence.Repositories.Guides;
using Voyara.Infrastructure.Persistence.Repository.Guides;

namespace Voyara.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Registers the F-18 guide services. It is a separate file so that no existing registration had to be edited;
    /// call it once from AddInfrastructure (see README).
    /// </summary>
    public static class GuidesInfrastructureExtensions
    {
        public static IServiceCollection AddGuidesInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<GuideDocumentOptions>()
                .Bind(configuration.GetSection(GuideDocumentOptions.SectionName));

            services.AddScoped<IGuideProfileRepository, GuideProfileRepository>();
            services.AddScoped<IGuideDocumentRepository, GuideDocumentRepository>();
            services.AddScoped<IGuideReadRepository, GuideReadRepository>();
            services.AddScoped<IGuideRoleGranter, GuideRoleGranter>();
            services.AddSingleton<IGuideDocumentPolicy, GuideDocumentPolicy>();

            return services;
        }
    }
}
