using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Starsbane.AI;

namespace AIClientSamples
{
    internal class VideoSample(
        ILogger<VideoSample> logger,
        IEnumerable<IAIClient> clients)
        : ISample
    {
        private string OutputPath => Path.Combine(Directory.GetCurrentDirectory(), "Generated");

        public async Task RunSample()
        {
            Directory.CreateDirectory(OutputPath);

            foreach (var client in clients)
            {
                logger.LogInformation($"Generating video...");
                var videoGenerationResponse =
                    await client.GetVideoGenerator().GenerateVideoAsync("A scientist is writing an astronomical theory.");
                var videoBytes = (videoGenerationResponse.Contents[0] as DataContent)?.Data.ToArray();
                var videoTempFilename =
                    Path.ChangeExtension(Path.Combine(OutputPath, $"{client.GetType().Name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}"), "mp4");
                await File.WriteAllBytesAsync(videoTempFilename, videoBytes!);
                logger.LogInformation($"Video file generated, the filename is {videoTempFilename}.");
            }
        }
    }
}
