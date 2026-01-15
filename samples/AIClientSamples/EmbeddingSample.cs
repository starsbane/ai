using Microsoft.Extensions.Logging;
using Starsbane.AI;

namespace AIClientSamples
{
    internal class EmbeddingSample(ILogger<EmbeddingSample> logger, IEnumerable<IAIClient> clients)
        : ISample
    {
        public async Task RunSample()
        {
            foreach (var client in clients)
            {
                logger.LogInformation($"Generating embeddings from text strings...");
                var embeddings = await client.GetEmbeddingGenerator().GenerateAsync(
                [
                    "Albert Einstein was a German-born theoretical physicist best known for developing the theory of relativity.",
                    "史蒂芬‧霍金(Stephen Hawking) 於1942年1月8日生於牛津",
                    "Ptolemy of Alexandria, arguably the greatest astronomer of antiquity, wrote a sweeping synthesis of the astronomical philosophy of the ancient Greeks.",
                    "Eugene ‘Gene’ Shoemaker is regarded as the founder of 'astrogeology'."
                ]);
                foreach (var e in embeddings)
                {
                    logger.LogInformation(
                        $"The embedding vector of sentence \"{e.AdditionalProperties!["OriginalString"]}\" is [{string.Join(",", e.Vector.ToArray().Take(5).ToArray())}, ...]");
                }

                logger.LogInformation(
                    $"Embeddings generated.");
            }
        }
    }
}
