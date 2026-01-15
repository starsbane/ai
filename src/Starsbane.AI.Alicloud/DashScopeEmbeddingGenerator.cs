using System.Text.RegularExpressions;
using Cnblogs.DashScope.AI;
using Cnblogs.DashScope.Core;
using Microsoft.Extensions.AI;

namespace Starsbane.AI.Alicloud
{
    public sealed class DashScopeEmbeddingGenerator(AlicloudAIClient parent) : BaseDashScopeSubClient(parent), IAIEmbeddingGenerator<string, Embedding<float>>
    {
        private readonly EmbeddingGeneratorMetadata _metadata = new(_providerName);

        public int EmbeddingTokenLimit => Parent.ClientOptions.Embedding?.TokenLimit ?? GetDefaultEmbeddingTokenLimit();

        private int GetDefaultEmbeddingTokenLimit()
        {
            if (Regex.Match(Parent.ClientOptions.Embedding?.ModelId ?? "", "text-embedding-v([0-9]+)") is { Success: true} match)
            {
                return int.Parse(match.Groups[1].Value) >= 3 ? 8192 : 2048;
            }

            return 2048;
        }


        public async Task<Embedding<float>> GenerateEmbeddingAsync(string value, EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = default)
        {
            return (await GenerateAsync([value], options, cancellationToken)).ElementAt(0);
        }

        public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(IEnumerable<string> values, EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = default)
        {
            Throw.IfNull(values);
            Throw.IfLessThan(values.Count(), 1);
            var mergedOptions = options.Merge(ClientOptions.Embedding);
            mergedOptions.ModelId = Throw.IfNullOrEmpty(mergedOptions.ModelId).ToLower();

            var allowedDimensionsValues = Array.Empty<int>();
            if (mergedOptions.ModelId.StartsWith("text-embedding-"))
            {
                if (mergedOptions.ModelId.EndsWith("v2") || mergedOptions.ModelId.EndsWith("v1"))
                {
                    allowedDimensionsValues = [1536];
                } 
                else if (mergedOptions.ModelId.EndsWith("v3"))
                {
                    allowedDimensionsValues = [1024, 768, 512, 256, 128, 64];
                }
                else
                {
                    allowedDimensionsValues = [2048, 1536, 1024, 768, 512, 256, 128, 64];
                }
            }
            else if (mergedOptions.ModelId.StartsWith("qwen") && mergedOptions.ModelId.EndsWith("-embedding"))
            {
                allowedDimensionsValues = [2048, 1024, 768, 512];
            }
            else if (mergedOptions.ModelId.StartsWith("tongyi-embedding-vision-plus"))
            {
                allowedDimensionsValues = [1152];
            }
            else if (mergedOptions.ModelId.StartsWith("tongyi-embedding-vision-flash"))
            {
                allowedDimensionsValues = [768];
            }
            else if (mergedOptions.ModelId.StartsWith("multimodal-embedding-"))
            {
                allowedDimensionsValues = [1024];
            }

            mergedOptions.Dimensions ??= (allowedDimensionsValues.Any()) 
                ? Throw.IfNotInEnumerable(options?.Dimensions ?? allowedDimensionsValues[0], allowedDimensionsValues) 
                : Throw.IfLessThan(mergedOptions.Dimensions ?? 1024, 1);

            using var generator = new DashScopeTextEmbeddingGenerator(PlatformClient, mergedOptions.ModelId, mergedOptions.Dimensions);
            var generatedEmbeddings = await generator.GenerateAsync(values, mergedOptions, cancellationToken);
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
                serviceType == typeof(DashScopeClient) ? PlatformClient :
                serviceType == typeof(EmbeddingGeneratorMetadata) ? _metadata :
                serviceType.IsInstanceOfType(this) ? this :
                null;
        }
    }
}
