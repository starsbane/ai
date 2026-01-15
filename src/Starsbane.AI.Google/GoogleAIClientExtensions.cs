using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Starsbane.AI.Google
{
    public static class GoogleAIClientExtensions
    {
        public static IServiceCollection AddGoogleAIClient(this IServiceCollection services, IConfiguration? configuration = null,
            Action<GoogleAIClientOptions>? configure = null)
        {
            Throw.IfNull(services);

            services.AddScoped<ISpeechFormatConverter, FFmpegSpeechFormatConverter>();
            services.AddScoped<IAIClient, GoogleAIClient>();
            services.AddScoped<IAIClient<GoogleAIClientOptions>, GoogleAIClient>();

            var optionsBuilder = services.AddOptions<GoogleAIClientOptions>();
            if (configuration != null)
            {
                optionsBuilder.Bind(configuration.GetSection(GoogleAIClientOptions.SectionName));
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
