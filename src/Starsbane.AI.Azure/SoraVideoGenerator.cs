using AzureSoraSDK;
using Microsoft.Extensions.AI;

namespace Starsbane.AI.Azure
{
    public sealed class SoraVideoGenerator : BaseSubClient<AzureAIClient, SoraClient, AzureAIClientOptions>, IVideoGenerator
    {
        private readonly VideoGeneratorMetadata _metadata = new("azure.sora");

        internal SoraVideoGenerator(AzureAIClient parent) : base(parent) =>
            PlatformClient = new SoraClient(
                Throw.IfNullOrEmpty(ClientOptions.AzureOpenAIEndpoint),
                Throw.IfNullOrEmpty(ClientOptions.AzureOpenAIApiKey),
                Throw.IfNullOrEmpty(ClientOptions.Video.DeploymentName));

        /// <inheritdoc/>
        public void Dispose()
        {
            PlatformClient.Dispose();
        }

        /// <inheritdoc/>
        public async Task<VideoGenerationResponse> GenerateAsync(VideoGenerationRequest request, VideoGenerationOptions? options = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Throw.IfNull(request);
            Throw.IfNullOrEmpty(request.Prompt);
            var mergedOptions = options.Merge(ClientOptions.Video);

            var width = Throw.NotInRangeOf(mergedOptions?.VideoDimensions?.Width ?? 1280, 480, 1920);
            var height = Throw.NotInRangeOf(mergedOptions?.VideoDimensions?.Height ?? 720, 480, 1920);
            var seconds = Throw.NotInRangeOf(mergedOptions?.Seconds ?? 10, 1, 20);

            string? newVideoPath = null;

            try
            {
                var jobId = await PlatformClient.SubmitVideoJobAsync(request.Prompt, width, height, seconds, cancellationToken);
                var videoUri = await PlatformClient.WaitForCompletionAsync(jobId, cancellationToken: cancellationToken);
                var videoPath = Path.GetTempFileName();
                newVideoPath = Path.ChangeExtension(videoPath, ".mp4");

                File.Move(videoPath, newVideoPath);
                await PlatformClient.DownloadVideoAsync(videoUri, newVideoPath, cancellationToken);
                var bytes = await File.ReadAllBytesAsync(newVideoPath, cancellationToken);

                return new VideoGenerationResponse(new List<AIContent>() { new DataContent(new ReadOnlyMemory<byte>(bytes), "video/mp4") });
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrEmpty(newVideoPath) && File.Exists(newVideoPath))
                        File.Delete(newVideoPath);
                }
                catch
                {
                    // ignored
                }
            }
        }

        /// <inheritdoc/>
        public object? GetService(Type serviceType, object? serviceKey = null)
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
