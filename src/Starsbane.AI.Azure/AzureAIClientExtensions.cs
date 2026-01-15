using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Starsbane.AI.Azure
{
    public static class AzureAIClientExtensions
    {
        public static IServiceCollection AddAzureAIClient(this IServiceCollection services, IConfiguration? configuration = null,
            Action<AzureAIClientOptions>? configure = null)
        {
            Throw.IfNull(services);

            services.AddScoped<IAIClient, AzureAIClient>();
            services.AddScoped<IAIClient<AzureAIClientOptions>, AzureAIClient>();
            var optionsBuilder = services.AddOptions<AzureAIClientOptions>();
            if (configuration != null)
            {
                optionsBuilder.Bind(configuration.GetSection(AzureAIClientOptions.SectionName));
            }

            optionsBuilder.ValidateOnStart();

            if (configure != null)
            {
                services.Configure(configure);
            }

            return services;
        }
    }
}
