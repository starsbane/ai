using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Starsbane.AI;

namespace AIClientSamples
{
    internal class SpeechSample(
        ILogger<SpeechSample> logger,
        IEnumerable<IAIClient> clients)
        : ISample
    {
        private string OutputPath => Path.Combine(Directory.GetCurrentDirectory(), "Generated");

        public async Task RunSample()
        {
            Directory.CreateDirectory(OutputPath);

            foreach (var client in clients)
            {
                logger.LogInformation($"Generating speech from text...");
                var speechGenerationResponse = await client.GetSpeechGenerator().GenerateSpeechAsync(
                    "Albert Einstein was a German-born theoretical physicist best known for developing the theory of relativity.");
                var speechBytes = (speechGenerationResponse.Contents[0] as DataContent)?.Data.ToArray();
                var speechTempFilename =
                    Path.ChangeExtension(Path.Combine(OutputPath, $"{client.GetType().Name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}"), "mp3");
                await File.WriteAllBytesAsync(speechTempFilename, speechBytes!);
                logger.LogInformation(
                    $"Speech file generated, the filename is {speechTempFilename}.");
                File.OpenRead(speechTempFilename);
            }
        }
    }
}
