using Amazon.BedrockRuntime;
using Microsoft.Extensions.AI;



namespace Starsbane.AI.Amazon
{
    public sealed class BedrockImageGenerator(AmazonAIClient parent) : BaseBedrockRuntimeSubClient(parent), IImageGenerator
    {
        private readonly ImageGeneratorMetadata _metadata = new(_providerName);

        /// <inheritdoc/>
        public async Task<ImageGenerationResponse> GenerateAsync(ImageGenerationRequest request, ImageGenerationOptions? options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            Throw.IfNullOrEmpty(request.Prompt);
            Throw.IfLessThan(options?.Count ?? 1, 1);
            Throw.IfGreaterThan(options?.Count ?? 1, 10);
            var mergedOptions = options.Merge(ClientOptions.Image);
            mergedOptions.ModelId = Throw.IfNullOrEmpty(mergedOptions.ModelId).ToLower();

            return await PlatformClient.AsIImageGenerator().GenerateAsync(request, options, cancellationToken);
        }

        /// <inheritdoc/>
        public object? GetService(Type serviceType, object? serviceKey = null)
        {
            Throw.IfNull(serviceType);

            return
                serviceKey is not null ? null :
                serviceType == typeof(ImageGeneratorMetadata) ? _metadata :
                serviceType.IsInstanceOfType(PlatformClient) ? PlatformClient :
                serviceType.IsInstanceOfType(this) ? this :
                null;
        }
    }
}
