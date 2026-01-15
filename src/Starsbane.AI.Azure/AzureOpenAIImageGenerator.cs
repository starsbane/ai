using Microsoft.Extensions.AI;
using OpenAI.Images;
using ImageGenerationOptions = OpenAI.Images.ImageGenerationOptions;



namespace Starsbane.AI.Azure
{
    public sealed class AzureOpenAIImageGenerator(AzureAIClient parent) : BaseAzureOpenAISubClient(parent), IImageGenerator
    {
        private readonly ImageGeneratorMetadata _metadata = new(_providerName);

        /// <inheritdoc/>
        public async Task<ImageGenerationResponse> GenerateAsync(ImageGenerationRequest request, Microsoft.Extensions.AI.ImageGenerationOptions? options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            Throw.IfNullOrEmpty(request.Prompt);
            Throw.IfLessThan(options?.Count ?? 1, 1);
            Throw.IfGreaterThan(options?.Count ?? 1, 10);
            var mergedOptions = options.Merge(ClientOptions.Image);
            var deploymentName = Throw.IfNullOrEmpty(mergedOptions?.ModelId ?? ClientOptions.Image?.DeploymentName).ToLower();

            ImageGenerationOptions imageGenerationOptions = new()
            {
                Size = options is { ImageSize: not null } ? new GeneratedImageSize(mergedOptions.ImageSize.Value.Width, mergedOptions.ImageSize.Value.Height) : GeneratedImageSize.W1024xH1024,
                Quality = GeneratedImageQuality.Standard,
                ResponseFormat = GeneratedImageFormat.Bytes,
                Style = GeneratedImageStyle.Natural,
                OutputFileFormat = GeneratedImageFileFormat.Jpeg
            };

            var imageClient = PlatformClient.GetImageClient(deploymentName);
            var result = await imageClient.GenerateImageAsync(request.Prompt, imageGenerationOptions, cancellationToken);

            return new ImageGenerationResponse(new List<AIContent>() { new DataContent(new ReadOnlyMemory<byte>(result.Value.ImageBytes.ToArray()), "image/jpeg") });
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
