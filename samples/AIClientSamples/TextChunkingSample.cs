using Microsoft.Extensions.Logging;
using Starsbane.AI;

namespace AIClientSamples
{
    internal class TextChunkingSample(
        ILogger<TextChunkingSample> logger,
        ISemanticChunker chunker)
        : ISample
    {
        public async Task RunSample()
        {
            logger.LogInformation($"Generating chunks from text string...");
            var chunks = await chunker.CreateChunksAsync(
                "Albert Einstein was a German-born theoretical physicist best known for developing the theory of relativity.");
            foreach (var c in chunks)
            {
                logger.LogInformation(
                    $"The embedding vector of chunk text \"{c.Text}\" is [{string.Join(",", c.Embedding.Vector.ToArray().Take(5).ToArray())}, ...]");
            }

            logger.LogInformation(
                $"Chunks generated from the text string.");
        }
    }
}
