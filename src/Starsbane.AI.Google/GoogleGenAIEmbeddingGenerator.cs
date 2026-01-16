using Google.GenAI.Types;
using Microsoft.Extensions.AI;


namespace Starsbane.AI.Google
{
    public sealed class GoogleGenAIEmbeddingGenerator(GoogleAIClient parent) : BaseGoogleGenAISubClient(parent), IAIEmbeddingGenerator<string, Embedding<float>>
    {
        private SpeechGeneratorMetadata _metadata => new(_providerName);

        /// <inheritdoc/>
        public async Task<Embedding<float>> GenerateEmbeddingAsync(string value,
            EmbeddingGenerationOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return new Embedding<float>((await GenerateAsync([value], options, cancellationToken))[0].Vector);
        }

        /// <inheritdoc/>
        public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(IEnumerable<string> values, EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            Throw.IfLessThan((values ?? []).Count(a => !string.IsNullOrEmpty(a)), 1);
            var mergedOptions = options.Merge(ClientOptions.Embedding);
            mergedOptions.ModelId = Throw.IfNullOrEmpty(mergedOptions.ModelId).ToLower();

            int dimensions;
            if (mergedOptions.ModelId.StartsWith("gemini-embedding-001"))
            {
                dimensions = Throw.NotInRangeOf(mergedOptions.Dimensions ?? 3072, 128, 3072);
            }
            else
            {
                dimensions = Throw.IfLessThan(mergedOptions.Dimensions ?? 1024, 1);
            }

            mergedOptions.Dimensions = dimensions;

            var generatedEmbeddings = await PlatformClient.AsIEmbeddingGenerator().GenerateAsync(values, mergedOptions, cancellationToken);
            for (var i = 0; i < values.Count(); i++)
            {
                var generatedEmbedding = generatedEmbeddings[i];
                generatedEmbedding.AdditionalProperties ??= new AdditionalPropertiesDictionary();
                generatedEmbedding.AdditionalProperties.Add("Platform", Parent.Platform);
                generatedEmbedding.AdditionalProperties.Add("OriginalString", values.ElementAt(i));
            }

            return generatedEmbeddings;
        }

        /// <inheritdoc/>
        public object? GetService(System.Type serviceType, object? serviceKey = null)
        {
            Throw.IfNull(serviceType);

            return
                serviceKey is not null ? null :
                serviceType == typeof(EmbeddingGeneratorMetadata) ? _metadata :
                serviceType.IsInstanceOfType(PlatformClient) ? PlatformClient :
                serviceType.IsInstanceOfType(this) ? this :
                null;
        }

        public int EmbeddingTokenLimit => Parent.ClientOptions?.Embedding?.TokenLimit ?? 2048;
    }
}
