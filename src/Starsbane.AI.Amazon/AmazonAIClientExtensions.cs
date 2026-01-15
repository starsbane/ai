using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Starsbane.AI.Amazon
{
    public static class AmazonAIClientExtensions
    {
        public static IServiceCollection AddAmazonAIClient(this IServiceCollection services, IConfiguration? configuration = null,
            Action<AmazonAIClientOptions>? configure = null)
        {
            Throw.IfNull(services);

            services.AddScoped<IAIClient, AmazonAIClient>();
            services.AddScoped<IAIClient<AmazonAIClientOptions>, AmazonAIClient>();
            var optionsBuilder = services.AddOptions<AmazonAIClientOptions>();
            if (configuration != null)
            {
                optionsBuilder.Bind(configuration.GetSection(AmazonAIClientOptions.SectionName));
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
