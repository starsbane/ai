using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime.Documents;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.AI;


namespace Starsbane.AI.Amazon
{
    public sealed class BedrockVideoGenerator(AmazonAIClient parent) : BaseBedrockRuntimeSubClient(parent), IVideoGenerator
    {
        private readonly VideoGeneratorMetadata _metadata = new(_providerName);

        /// <inheritdoc/>
        public async Task<VideoGenerationResponse> GenerateAsync(VideoGenerationRequest request, VideoGenerationOptions? options = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Throw.IfNull(request);
            Throw.IfNullOrEmpty(request.Prompt);
            Throw.IfNullOrEmpty(Parent.ClientOptions.Video?.VideoOutputS3Uri.AbsoluteUri);
            var mergedOptions = options.Merge(ClientOptions.Video);
            mergedOptions.ModelId = Throw.IfNullOrEmpty(mergedOptions.ModelId).ToLower();

            var width = Throw.NotEqualTo(mergedOptions?.VideoDimensions?.Width ?? 1280, 1280);
            var height = Throw.NotEqualTo(mergedOptions?.VideoDimensions?.Height ?? 720, 720);
            var seconds = Throw.NotEqualTo(mergedOptions?.Seconds ?? 6, 6);
            var fps = Throw.NotEqualTo(mergedOptions?.FPS ?? 24, 24);

            string? tempPath = null;
            try
            {
                var response = await PlatformClient.StartAsyncInvokeAsync(new StartAsyncInvokeRequest()
                {
                    ModelId = mergedOptions.ModelId,
                    ModelInput = new Document()
                    {
                        { "taskType", "TEXT_VIDEO" },
                        {
                            "textToVideoParams", new Document
                            {
                                { "text", request.Prompt }
                            }
                        },
                        {
                            "videoGenerationConfig", new Document
                            {
                                { "durationSeconds", seconds }, // Single-shot video duration
                                { "fps", fps },
                                { "dimension", $"{width}X{height}" }
                            }
                        }
                    },
                    OutputDataConfig = new()
                    {
                        S3OutputDataConfig = new()
                        {
                            S3Uri = Parent.ClientOptions.Video?.VideoOutputS3Uri.AbsoluteUri
                        }
                    }
                }, cancellationToken);

                var invocationArn = response.InvocationArn;
                var videoJobMonitoringTask = Task.Run(async () =>
                {
                    while (true)
                    {
                        var job = await PlatformClient.GetAsyncInvokeAsync(new()
                        {
                            InvocationArn = invocationArn
                        }, cancellationToken);

                        if (job.Status == AsyncInvokeStatus.InProgress)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                        }
                        else if (job.Status == AsyncInvokeStatus.Failed)
                        {
                            throw new ApplicationException(
                                $"Failed to call {nameof(IVideoGenerator.GenerateAsync)}: {job.FailureMessage}");
                        }

                        return job.OutputDataConfig.S3OutputDataConfig.S3Uri + "/output.mp4";
                    }
                }, cancellationToken);

                var videoUrl = await videoJobMonitoringTask;
                var (bucketName, keyName) = S3UriParser.ParseS3Uri(videoUrl);

                using var s3Client = Parent.AWSOptions.CreateServiceClient<IAmazonS3>();
                var transferUtility = new TransferUtility(s3Client);

                tempPath = Path.GetTempFileName();
                await transferUtility.DownloadAsync(tempPath, bucketName, keyName, cancellationToken);

                using var fileStream = File.OpenRead(tempPath);
                using var ms = new MemoryStream();
                await fileStream.CopyToAsync(ms);
                var bytes = ms.ToArray();

                return new VideoGenerationResponse(new List<AIContent>() { new DataContent(new ReadOnlyMemory<byte>(bytes), "video/mp4") });
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrEmpty(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }
                catch
                {
                    // ignore
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
