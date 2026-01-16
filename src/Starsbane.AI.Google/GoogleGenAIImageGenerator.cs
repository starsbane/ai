using Microsoft.Extensions.AI;



namespace Starsbane.AI.Google
{
    public sealed class GoogleGenAIImageGenerator(GoogleAIClient parent) : BaseGoogleGenAISubClient(parent), IImageGenerator
    {
        private ImageGeneratorMetadata _metadata => new(_providerName);

        /// <inheritdoc/>
        public async Task<ImageGenerationResponse> GenerateAsync(ImageGenerationRequest request, Microsoft.Extensions.AI.ImageGenerationOptions? options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            Throw.IfNullOrEmpty(request.Prompt);
            Throw.IfLessThan(options?.Count ?? 1, 1);
            Throw.IfGreaterThan(options?.Count ?? 1, 10);
            var mergedOptions = options.Merge(ClientOptions.Image);
            mergedOptions.ModelId = Throw.IfNullOrEmpty(mergedOptions.ModelId).ToLower();

            return await PlatformClient.AsIImageGenerator().GenerateAsync(request, options, cancellationToken);
        }

        /// <inheritdoc/>
        public object? GetService(System.Type serviceType, object? serviceKey = null)
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
