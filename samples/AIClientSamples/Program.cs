using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starsbane.AI.Alicloud;
using Starsbane.AI.Amazon;
using Starsbane.AI.Azure;
using Starsbane.AI.Chunking;
using Starsbane.AI.Google;

namespace AIClientSamples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.user.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSemanticChunkerNET();
                    services.AddPdfPigExtractor();

                    //services.AddTransient<ISample, ChatSample>();
                    //services.AddTransient<ISample, DocumentExtractionSample>();
                    //services.AddTransient<ISample, EmbeddingSample>();
                    //services.AddTransient<ISample, ImageSample>();
                    //services.AddTransient<ISample, PDFChunkingSample>();
                    services.AddTransient<ISample, SpeechSample>();
                    //services.AddTransient<ISample, TextChunkingSample>();
                    //services.AddTransient<ISample, TextExtractionSample>();
                    //services.AddTransient<ISample, VideoSample>();

                    //services.AddAzureAIClient(context.Configuration);
                    //services.AddAlicloudAIClient(context.Configuration);
                    //services.AddAmazonAIClient(context.Configuration);
                    services.AddGoogleAIClient(context.Configuration);
                });

            using IHost host = hostBuilder.Build();
            var clientServices = host.Services.GetServices<ISample>();
            foreach (var clientService in clientServices)
            {
                await clientService.RunSample();
            }
        }
    }
}
