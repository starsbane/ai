using Amazon.BedrockRuntime;
using Microsoft.Extensions.AI;

namespace Starsbane.AI.Amazon
{
    public sealed class BedrockEmbeddingGenerator(AmazonAIClient parent) : BaseBedrockRuntimeSubClient(parent), IAIEmbeddingGenerator<string, Embedding<float>>
    {
        private readonly EmbeddingGeneratorMetadata _metadata = new(_providerName);

        /// <inheritdoc/>
        public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(IEnumerable<string> values, EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            Throw.IfNull(values);
            Throw.IfLessThan(values.Count(a => !string.IsNullOrEmpty(a)), 1);
            var mergedOptions = options.Merge(ClientOptions.Embedding);
            mergedOptions.ModelId = Throw.IfNullOrEmpty(mergedOptions.ModelId).ToLower();

            var allowedDimensionsValues = Array.Empty<int>();
            if (mergedOptions.ModelId.StartsWith("amazon.titan-embed-text-v2"))
            {
                allowedDimensionsValues = [1024, 512, 256];
            }
            else if (mergedOptions.ModelId.StartsWith("amazon.titan-embed-image-v1"))
            {
                allowedDimensionsValues = [1024, 384, 256];
            }
            else if (mergedOptions.ModelId.StartsWith("cohere.embed-v4"))
            {
                allowedDimensionsValues = [1536, 1024, 512, 256];
            }

            mergedOptions.Dimensions ??= (allowedDimensionsValues.Any()) ? 
                Throw.IfNotInEnumerable(mergedOptions.Dimensions ?? allowedDimensionsValues[0], allowedDimensionsValues) : 
                Throw.IfLessThan(mergedOptions.Dimensions ?? 1024, 1);

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
        public async Task<Embedding<float>> GenerateEmbeddingAsync(string value,
            EmbeddingGenerationOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return new Embedding<float>((await GenerateAsync([value], options, cancellationToken))[0].Vector);
        }

        public int EmbeddingTokenLimit => Parent.ClientOptions?.Embedding?.TokenLimit ?? 8192;

        /// <inheritdoc/>
        public object? GetService(Type serviceType, object? serviceKey = null)
        {
            Throw.IfNull(serviceType);

            return
                serviceKey is not null ? null :
                serviceType == typeof(EmbeddingGeneratorMetadata) ? _metadata :
                serviceType.IsInstanceOfType(PlatformClient) ? PlatformClient :
                serviceType.IsInstanceOfType(this) ? this :
                null;
        }
    }
}
