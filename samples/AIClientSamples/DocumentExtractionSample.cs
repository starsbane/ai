using Microsoft.Extensions.Logging;
using Starsbane.AI;
using System.Net.Mime;

namespace AIClientSamples
{
    internal class DocumentExtractionSample(ILogger<DocumentExtractionSample> logger, IEnumerable<IAIClient> clients) : ISample
    {
        public async Task RunSample()
        {
            foreach (var client in clients)
            {
                var sampleFile = Path.Combine(Directory.GetCurrentDirectory(), "document_extraction_1.jpg");
                try
                {
                    logger.LogInformation(
                        $"Extracting text from document {Path.GetFileName(sampleFile)}...");

                    var fileBytes = await BinaryData.FromFileAsync(sampleFile, MediaTypeNames.Image.Jpeg);
                    var extractedText = await client.GetDocumentExtractionClient().ExtractFromDocument(fileBytes);

                    logger.LogInformation(
                        $"Text extracted from document {Path.GetFileName(sampleFile)}: {extractedText}");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Text extraction from document failed. Message: {ex.Message}");
                }
            }
        }
    }
}
