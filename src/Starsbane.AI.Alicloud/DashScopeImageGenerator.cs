using Cnblogs.DashScope.Core;
using Cnblogs.DashScope.Sdk.Wanx;
using Microsoft.Extensions.AI;



namespace Starsbane.AI.Alicloud
{
    public sealed class DashScopeImageGenerator(AlicloudAIClient parent) : BaseDashScopeSubClient(parent), IImageGenerator
    {
        private readonly ImageGeneratorMetadata _metadata = new(_providerName);

        private static async Task<ImageGenerationResponse> handleImageSynthesisOutput(ImageSynthesisOutput output, CancellationToken cancellationToken)
        {
            if (output.TaskStatus == DashScopeTaskStatus.Succeeded)
            {
                var imagesBytes = new List<byte[]>();

                // Download generated images
                using var httpClient = new HttpClient();
                foreach (var imageResult in output.Results ?? new List<ImageSynthesisResult>())
                {
                    using var imageResponse = await httpClient.GetAsync(imageResult.Url, cancellationToken);
                    imagesBytes.Add(await imageResponse.Content.ReadAsByteArrayAsync(cancellationToken));
                }

                var contents = imagesBytes.Select(bytes =>
                    new DataContent(new ReadOnlyMemory<byte>(bytes), "image/png") as AIContent).ToList();
                return new ImageGenerationResponse(contents);
            }

            throw new InvalidOperationException($"Image generation failed.{Environment.NewLine}Code: {output.Code}{Environment.NewLine}Message:{output.Message}");
        }

        /// <inheritdoc/>
        public async Task<ImageGenerationResponse> GenerateAsync(ImageGenerationRequest request, ImageGenerationOptions? options = null, CancellationToken cancellationToken = default)
        {
            Throw.IfNullOrEmpty(request.Prompt);
            Throw.IfLessThan(options?.Count ?? 1, 1);
            Throw.IfGreaterThan(options?.Count ?? 1, 10);
            var mergedOptions = options.Merge(ClientOptions.Image);
            mergedOptions.ModelId = Throw.IfNullOrEmpty(mergedOptions.ModelId).ToLower();

            if (mergedOptions.ModelId.StartsWith("wan"))
            {
                var task = await PlatformClient.CreateWanxImageSynthesisTaskAsync(mergedOptions.ModelId, request.Prompt, null, (ImageSynthesisParameters)ToImageSynthesisParameters(options)!, cancellationToken);

                while (true)
                {
                    var taskResult = await PlatformClient.GetWanxImageSynthesisTaskAsync(task.TaskId, cancellationToken);

                    if (taskResult.Output.TaskStatus is DashScopeTaskStatus.Running or DashScopeTaskStatus.Pending or DashScopeTaskStatus.Unknown)
                    {
                        await Task.Delay(10 * 1000, cancellationToken);
                    }
                    else
                    {
                        return await handleImageSynthesisOutput(taskResult.Output, cancellationToken);
                    }
                }
            }

            var modelRequest = new ModelRequest<ImageSynthesisInput, IImageSynthesisParameters>();
            modelRequest.Parameters = ToImageSynthesisParameters(mergedOptions);

            var modelResponse = await PlatformClient.CreateImageSynthesisTaskAsync(null, cancellationToken);
            return await handleImageSynthesisOutput(modelResponse.Output, cancellationToken);
        }

        private IImageSynthesisParameters? ToImageSynthesisParameters(ImageGenerationOptions? options)
        {
            if (options == null) return null;
            if (options?.RawRepresentationFactory?.Invoke(this) is not IImageSynthesisParameters result)
            {
                result = new ImageSynthesisParameters
                {
                    Style = null,
                    Size = (!options!.ImageSize.HasValue)
                        ? null
                        : $"{options.ImageSize.Value.Width}*{options.ImageSize.Value.Height}",
                    N = options?.Count,
                    Seed = null,
                    PromptExtend = null,
                    Watermark = null,
                };
            }

            Throw.IfLessThan(result.N ?? 1, 1);

            return result;
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
