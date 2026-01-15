using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Starsbane.AI;
using System.Net.Mime;

namespace AIClientSamples
{
    internal class PDFChunkingSample(
        ILogger<PDFChunkingSample> logger,
        IPdfFileChunkExtractor pdfFileChunkExtractor)
        : ISample
    {
        private string OutputPath => Path.Combine(Directory.GetCurrentDirectory(), "Generated");

        public async Task RunSample()
        {
            Directory.CreateDirectory(OutputPath);

            var file = Path.Combine(Directory.GetCurrentDirectory(), "pdf_chunking_1.pdf");
            try
            {
                logger.LogInformation($"Generating chunks from PDF files...");
                var chunks =
                    await pdfFileChunkExtractor.ExtractChunksAsync(file);
                foreach (var c in chunks)
                {
                    logger.LogInformation(
                        $"The embedding vector of sentence \"{c.Text}\" is [{string.Join(",", c.Embedding.Vector.ToArray().Take(5).ToArray())}, ...]");
                }
                logger.LogInformation(
                    $"Chunks generated from files.");
            }
            catch (Exception ex)
            {
                logger.LogError($"PDF chunking failed. Message: {ex.Message}");
            }
        }
    }
}
