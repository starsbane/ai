using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Starsbane.AI;

namespace AIClientSamples
{
    internal class ImageSample(ILogger<ImageSample> logger, IEnumerable<IAIClient> clients)
        : ISample
    {
        private string OutputPath => Path.Combine(Directory.GetCurrentDirectory(), "Generated");

        public async Task RunSample()
        {
            foreach (var client in clients)
            {
                logger.LogInformation($"Generating image...");

                var imageGenerationResponse =
                    await client.GetImageGenerator().GenerateImagesAsync("A fantasy landscape with castles and dragons.");
                var imageBytes = (imageGenerationResponse.Contents[0] as DataContent)?.Data.ToArray();
                var imageTempFilename =
                    Path.ChangeExtension(
                        Path.Combine(OutputPath,
                            $"{client.GetType().Name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}"),
                        "jpg");
                await File.WriteAllBytesAsync(imageTempFilename, imageBytes!);
                logger.LogInformation(
                    $"Image generated, the filename is {imageTempFilename}.");
                File.OpenRead(imageTempFilename);
            }
        }
    }
}
