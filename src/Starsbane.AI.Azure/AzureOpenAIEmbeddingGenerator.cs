using Azure.Core;
using Microsoft.Extensions.AI;




namespace Starsbane.AI.Azure
{
    public sealed class AzureOpenAIEmbeddingGenerator(AzureAIClient parent) : BaseAzureOpenAISubClient(parent), IAIEmbeddingGenerator<string, Embedding<float>>
    {
        private readonly EmbeddingGeneratorMetadata _metadata = new(_providerName);

        /// <inheritdoc/>
        public async Task<Embedding<float>> GenerateEmbeddingAsync(string value,
            Microsoft.Extensions.AI.EmbeddingGenerationOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return new Embedding<float>((await GenerateAsync([value], options, cancellationToken))[0].Vector);
        }

        /// <inheritdoc/>
        public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(IEnumerable<string> values,
            Microsoft.Extensions.AI.EmbeddingGenerationOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Throw.IfNull(values);
            Throw.IfLessThan(values.Count(a => !string.IsNullOrEmpty(a)), 1);
            var mergedOptions = options.Merge(ClientOptions.Embedding);
            var deploymentName = Throw.IfNullOrEmpty(mergedOptions?.ModelId ?? ClientOptions.Embedding?.DeploymentName).ToLower();

            var embeddingClient = PlatformClient.GetEmbeddingClient(deploymentName);
            var dimensions = mergedOptions.Dimensions ?? 1536;
            if (!string.IsNullOrEmpty(embeddingClient.Model))
            {
                dimensions = embeddingClient.Model.ToLower() switch
                {
                    "text-embedding-ada-002" => Throw.NotEqualTo(dimensions, 1536),
                    "text-embedding-3-large" => Throw.NotInRangeOf(dimensions, 1, 3072),
                    "text-embedding-3-small" => Throw.NotInRangeOf(dimensions, 1, 1536),
                    _ => Throw.IfLessThan(dimensions, 1)
                };
            }

            mergedOptions.Dimensions ??= dimensions;

            var generatedEmbeddings = await embeddingClient.AsIEmbeddingGenerator(dimensions).GenerateAsync(values, mergedOptions, cancellationToken);
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

        public int EmbeddingTokenLimit => Parent.ClientOptions?.Embedding?.TokenLimit ?? 8192;
    }
}
