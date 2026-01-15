using Google.GenAI.Types;
using Microsoft.Extensions.AI;
using Environment = System.Environment;



namespace Starsbane.AI.Google
{
    public sealed class GoogleGenAIVideoGenerator(GoogleAIClient parent) : BaseGoogleGenAISubClient(parent), IVideoGenerator
    {
        private VideoGeneratorMetadata _metadata => new(_providerName);

        /// <inheritdoc/>
        public async Task<VideoGenerationResponse> GenerateAsync(VideoGenerationRequest request, VideoGenerationOptions? options = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Throw.IfNull(request);
            Throw.IfNullOrEmpty(request.Prompt);
            var mergedOptions = options.Merge(ClientOptions.Video);
            var height = mergedOptions?.VideoDimensions?.Height ?? 720;
            var fps = Throw.NotInRangeOf(mergedOptions?.FPS ?? 2, 1, 24);

            mergedOptions.ModelId = Throw.IfNullOrEmpty(mergedOptions.ModelId).ToLower();
            if (mergedOptions.Seconds.HasValue)
            {
                if (mergedOptions.ModelId.StartsWith("veo-"))
                {
                    Throw.IfNotInEnumerable(mergedOptions.Seconds, [4, 5, 6, 8]);
                }
                else
                {
                    Throw.NotInRangeOf(mergedOptions.Seconds.Value, 1, 141);
                }
            }

            Throw.IfNullOrEmpty(request.Prompt);
            if (height != 720 && height != 1080)
            {
                throw new ArgumentOutOfRangeException(nameof(mergedOptions.VideoDimensions.Value.Height),
                    "Video resolution must be either 720p or 1080p");
            }
            Throw.IfLessThan(fps, 1);

            if (mergedOptions.ModelId.StartsWith("veo-"))
            {
                GenerateVideosSource source = new()
                {
                    Prompt = request.Prompt,
                };

                GenerateVideosConfig config = new()
                {
                    NumberOfVideos = 1,
                    Fps = ClientOptions.IsVertex ? fps : null,
                    DurationSeconds = mergedOptions?.Seconds,
                    Resolution = $"{height}p",
                };

                var operation = await PlatformClient.Models.GenerateVideosAsync(model: mergedOptions.ModelId, source, config);

                while (operation.Done != true)
                {
                    await Task.Delay(10000, cancellationToken);
                    operation = await PlatformClient.Operations.GetAsync(operation, null);
                }


                if (operation.Response?.RaiMediaFilteredReasons != null && operation.Response.RaiMediaFilteredReasons.Any())
                {
                    throw new InvalidOperationException($"Video generation request filtered: {string.Join(Environment.NewLine, operation.Response.RaiMediaFilteredReasons)}");
                }

                var bytes = operation.Response?.GeneratedVideos?.First()?.Video?.VideoBytes;
                if (bytes == null)
                {
                    using var fileStream = await PlatformClient.Files.DownloadAsync(operation.Response?.GeneratedVideos?.First()?.Video);
                    using var memoryStream = new MemoryStream();
                    await fileStream.CopyToAsync(memoryStream);
                    bytes = memoryStream.ToArray();
                }

                return new VideoGenerationResponse(new List<AIContent>() { new DataContent(new ReadOnlyMemory<byte>(bytes), "video/mp4") });
            }

            throw new InvalidOperationException("Non Veo model is currently unsupported for video generation.");
        }

        /// <inheritdoc/>
        public object? GetService(System.Type serviceType, object? serviceKey = null)
        {
            Throw.IfNull(serviceType);

            return
                serviceKey is not null ? null :
                serviceType == typeof(VideoGeneratorMetadata) ? _metadata :
                serviceType.IsInstanceOfType(PlatformClient) ? PlatformClient :
                serviceType.IsInstanceOfType(this) ? this :
                null;
        }
    }
}
