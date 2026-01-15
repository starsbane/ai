using System.Net.Mime;
using Microsoft.Extensions.Logging;
using Starsbane.AI;

namespace AIClientSamples
{
    internal class TextExtractionSample(ILogger<TextExtractionSample> logger, IEnumerable<IAIClient> clients) : ISample
    {
        public async Task RunSample()
        {
            foreach (var client in clients)
            {
                var sampleFile = Path.Combine(Directory.GetCurrentDirectory(), "text_extraction_1.png");
                try
                {
                    logger.LogInformation(
                        $"Extracting text from {Path.GetFileName(sampleFile)}...");

                    var fileBytes = await BinaryData.FromFileAsync(sampleFile, MediaTypeNames.Image.Png);
                    var extractedText = await client.GetOCRClient().ExtractFromImage(fileBytes);

                    logger.LogInformation(
                        $"Text extracted from {Path.GetFileName(sampleFile)}: {extractedText}");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Text extraction failed. Message: {ex.Message}");
                }
            }
        }
    }
}
