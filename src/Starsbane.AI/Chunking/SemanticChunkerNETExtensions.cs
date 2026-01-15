using Microsoft.Extensions.DependencyInjection;

namespace Starsbane.AI.Chunking
{
    public static class SemanticChunkerNETExtensions
    {
        public static IServiceCollection AddSemanticChunkerNET(this IServiceCollection services)
        {
            services.AddScoped<ISemanticChunker, SemanticChunkerNET>();
            return services;
        }
    }
}