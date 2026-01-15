using Google.GenAI.Types;
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

            if (mergedOptions.ModelId.StartsWith("imagen-"))
            {
                GenerateImagesConfig config = new()
                {
                    NumberOfImages = options?.Count ?? 1,
                    AspectRatio = "1:1",
                    OutputMimeType = ClientOptions.IsVertex ? "image/jpeg" : null,
                    ImageSize = options is { ImageSize: not null } ? options.ImageSize.Value.ToString() : null
                };

                var response = await PlatformClient.Models.GenerateImagesAsync(mergedOptions.ModelId, request.Prompt, config);
                return new ImageGenerationResponse(new List<AIContent>() { new DataContent(new ReadOnlyMemory<byte>(response.GeneratedImages?.First()?.Image?.ImageBytes), "image/jpeg") });
            }
            else
            {
                GenerateContentConfig config = new()
                {
                    ImageConfig = new()
                    {
                        AspectRatio = "1:1",
                        ImageSize = options is { ImageSize: not null } ? options.ImageSize.Value.ToString() : null,
                        OutputMimeType = ClientOptions.IsVertex ? "image/jpeg" : null,
                    }
                };

                var response = await PlatformClient.Models.GenerateContentAsync(mergedOptions.ModelId, request.Prompt, config);
                return new ImageGenerationResponse(new List<AIContent>() { new DataContent(new ReadOnlyMemory<byte>(response.Candidates?.First()?.Content?.Parts?.First()?.InlineData?.Data), "image/jpeg") });
            }
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
