using Microsoft.Extensions.DependencyInjection;

namespace Starsbane.AI.Chunking
{
    public static class PdfPigExtractorExtensions
    {
        public static IServiceCollection AddPdfPigExtractor(this IServiceCollection services)
        {
            services.AddScoped<IPdfFileChunkExtractor, PdfPigExtractor>();
            return services;
        }
    }
}
