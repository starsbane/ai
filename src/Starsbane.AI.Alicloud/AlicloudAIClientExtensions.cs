using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Starsbane.AI.Alicloud
{
    public static class AlicloudAIClientExtensions
    {
        public static IServiceCollection AddAlicloudAIClient(this IServiceCollection services, IConfiguration? configuration = null,
            Action<AlicloudAIClientOptions>? configure = null)
        {
            Throw.IfNull(services);

            services.AddScoped<IAIClient, AlicloudAIClient>();
            services.AddScoped<IAIClient<AlicloudAIClientOptions>, AlicloudAIClient>();
            var optionsBuilder = services.AddOptions<AlicloudAIClientOptions>();

            if (configuration != null)
            {
                optionsBuilder.Bind(configuration.GetSection(AlicloudAIClientOptions.SectionName));
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
