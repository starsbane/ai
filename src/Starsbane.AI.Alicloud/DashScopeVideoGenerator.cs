using Cnblogs.DashScope.Core;
using Microsoft.Extensions.AI;
using System.Drawing;
using System.Text.Json.Serialization;



namespace Starsbane.AI.Alicloud
{
    public sealed class DashScopeVideoGenerator(AlicloudAIClient parent) : BaseDashScopeSubClient(parent), IVideoGenerator
    {
        private readonly VideoGeneratorMetadata _metadata = new(_providerName);
        private static async Task<VideoGenerationResponse> handleVideoSynthesisOutput(VideoSynthesisOutput output, CancellationToken cancellationToken)
        {
            if (output.TaskStatus == DashScopeTaskStatus.Succeeded)
            {
                var imagesBytes = new List<byte[]>();

                // Download generated images
                using var httpClient = new HttpClient();
                using var imageResponse = await httpClient.GetAsync(output.VideoUrl, cancellationToken);
                imagesBytes.Add(await imageResponse.Content.ReadAsByteArrayAsync(cancellationToken));

                var contents = imagesBytes.Select(bytes =>
                    new DataContent(new ReadOnlyMemory<byte>(bytes), "image/png") as AIContent).ToList();
                return new VideoGenerationResponse(contents);
            }

            throw new InvalidOperationException($"Video generation failed.{Environment.NewLine}Code: {output.Code}{Environment.NewLine}Message:{output.Message}");
        }

        private static readonly IList<Size> Sizes480P =
        [
            VideoSize.Get("832*480"),
            VideoSize.Get("480*832"),
            VideoSize.Get("624*624")
        ];

        private static readonly IList<Size> Sizes720P =
        [
            VideoSize.Get("1280*720"),
            VideoSize.Get("720*1280"),
            VideoSize.Get("960*960"),
            VideoSize.Get("1088*832"),
            VideoSize.Get("832*1088")
        ];

        private static readonly IList<Size> Sizes1080P =
        [
            VideoSize.Get("1920*1080"),
            VideoSize.Get("1080*1920"),
            VideoSize.Get("1440*1440"),
            VideoSize.Get("1632*1248"),
            VideoSize.Get("1248*1632")
        ];

        /// <inheritdoc/>
        public async Task<VideoGenerationResponse> GenerateAsync(VideoGenerationRequest request, VideoGenerationOptions? options = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Throw.IfNull(request);
            Throw.IfNullOrEmpty(request.Prompt);
            var mergedOptions = options.Merge(ClientOptions.Video);

            mergedOptions.ModelId = Throw.IfNullOrEmpty(mergedOptions.ModelId).ToLower();

            Size defaultVideoSize = default;
            int defaultDurarion = 0;
            var allowedSizes = new List<Size>();
            var allowedDurations = Array.Empty<int>();

            if (mergedOptions.ModelId.StartsWith("wan2.6-t2v"))
            {
                defaultVideoSize = Sizes1080P[0];
                allowedSizes.AddRange(Sizes720P);
                allowedSizes.AddRange(Sizes1080P);
                defaultDurarion = 5;
                allowedDurations = [5, 10, 15];
            } 
            else if (mergedOptions.ModelId.StartsWith("wan2.5-t2v"))
            {
                defaultVideoSize = Sizes1080P[0];
                allowedSizes.AddRange(Sizes480P);
                allowedSizes.AddRange(Sizes720P);
                allowedSizes.AddRange(Sizes1080P);
                defaultDurarion = 5;
                allowedDurations = [5, 10];
            }
            else if (mergedOptions.ModelId.StartsWith("wan2.2-t2v-plus"))
            {
                defaultVideoSize = Sizes1080P[0];
                allowedSizes.AddRange(Sizes480P);
                allowedSizes.AddRange(Sizes1080P);
                defaultDurarion = 5;
                allowedDurations = [5];
            }
            else if (mergedOptions.ModelId.StartsWith("wanx2.1-t2v-turbo"))
            {
                defaultVideoSize = Sizes720P[0];
                allowedSizes.AddRange(Sizes480P);
                allowedSizes.AddRange(Sizes720P);
                defaultDurarion = 5;
                allowedDurations = [5];
            }
            else if (mergedOptions.ModelId.StartsWith("wanx2.1-t2v-plus"))
            {
                defaultVideoSize = Sizes720P[0];
                allowedSizes.AddRange(Sizes720P);
                defaultDurarion = 5;
                allowedDurations = [5];
            }
            else
            {
                throw new ArgumentException($"Unsupported model: {mergedOptions.ModelId}", nameof(mergedOptions.ModelId));
            }

            var videoSize = Throw.IfNotInEnumerable(mergedOptions?.VideoDimensions ?? defaultVideoSize, allowedSizes);
            var seconds = Throw.IfNotInEnumerable(mergedOptions?.Seconds ?? defaultDurarion, allowedDurations);

            var dashScopeInput = new ModelRequest<VideoSynthesisInput, VideoSynthesisParameters>
            {
                Model = mergedOptions.ModelId,
                Input = new VideoSynthesisInput()
                {
                    Prompt = request.Prompt
                },
                Parameters = new VideoSynthesisParameters()
                {
                    Duration = seconds,
                    Size = $"{videoSize.Width}*{videoSize.Height}",
                }
            };
            var dashScopeRequest = BuildRequest(HttpMethod.Post, "services/aigc/video-generation/video-synthesis", dashScopeInput, isTask: true);
            var task = (await SendAsync<ModelResponse<VideoSynthesisOutput, VideoSynthesisUsage>>(dashScopeRequest, cancellationToken))!.Output;

            while (true)
            {
                var taskResult = await PlatformClient.GetTaskAsync<VideoSynthesisOutput, VideoSynthesisUsage>(task.TaskId, cancellationToken);

                if (taskResult.Output.TaskStatus is DashScopeTaskStatus.Running or DashScopeTaskStatus.Pending or DashScopeTaskStatus.Unknown)
                {
                    await Task.Delay(10 * 1000, cancellationToken);
                }
                else
                {
                    return await handleVideoSynthesisOutput(taskResult.Output, cancellationToken);
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

        internal class VideoSynthesisInput
        {
            /// <summary>
            /// The prompt to generate image from. This will be chopped at max length of 500 characters.
            /// </summary>
            [JsonPropertyName("prompt")]
            public string Prompt { get; set; } = string.Empty;

            /// <summary>
            /// The negative prompt to generate image from. This will be chopped at max length of 500 characters.
            /// </summary>
            [JsonPropertyName("negative_prompt")]
            public string? NegativePrompt { get; set; }

            /// <summary>
            /// 音频文件URL，模型将使用该音频生成视频。使用方法参见设置音频参数。
            /// 支持 HTTP 或 HTTPS 协议。本地文件可通过上传文件获取临时URL。
            /// </summary>
            [JsonPropertyName("audio_url")]
            public Uri? AudioUrl { get; set; }
        }

        internal class VideoSynthesisParameters
        {
            /// <summary>
            /// 指定生成的视频分辨率，格式为宽*高。该参数的默认值和可用枚举值依赖于 model 参数
            /// </summary>
            [JsonPropertyName("size")]
            public string? Size { get; set; }

            /// <summary>
            /// 生成视频的时长，单位为秒。该参数的取值依赖于 model参数
            /// </summary>
            [JsonPropertyName("duration")]
            public int? Duration { get; set; }

            /// <summary>
            /// Let LLM to rewrite your positive prompt, Defaults to true.
            /// </summary>
            [JsonPropertyName("prompt_extend")]
            public bool? PromptExtend { get; set; }

            /// <summary>
            /// 指定生成视频的镜头类型，即视频是由一个连续镜头还是多个切换镜头组成。
            /// </summary>
            [JsonPropertyName("shot_type")]
            public string? ShotType { get; set; }

            /// <summary>
            /// Adds AI-Generated watermark on bottom right corner.
            /// </summary>
            [JsonPropertyName("watermark")]
            public bool? Watermark { get; set; }

            /// <summary>
            /// 随机数种子，取值范围为[0, 2147483647]。
            /// 未指定时，系统自动生成随机种子。若需提升生成结果的可复现性，建议固定seed值。
            /// </summary>
            [JsonPropertyName("seed")]
            public uint? Seed { get; set; }
        }

        /// <summary>
        /// The output of a single video synthesis task.
        /// </summary>
        internal record VideoSynthesisOutput : DashScopeTaskOutput
        {
            /// <summary>
            /// The url of generated video.
            /// </summary>
            [JsonPropertyName("video_url")]
            public string? VideoUrl { get; set; }
        }

        /// <summary>
        /// The usage of one image synthesis request.
        /// </summary>
        /// <param name="VideoDuration">仅在使用 wan2.5 及以下版本模型时返回，用于计费。生成视频的时长，单位秒。枚举值为5、10。</param>
        /// <param name="Duration">仅在使用 wan2.6 模型时返回，用于计费。表示总的视频时长，</param>
        /// <param name="InputVideoDuration">仅在使用 wan2.6 模型时返回。固定为0</param>
        /// <param name="OutputVideoDuration">仅在使用 wan2.6 模型时返回。输出视频的时长，单位秒。其值等同于input.duration的值。</param>
        /// <param name="SR">仅在使用 wan2.6 模型时返回。生成视频的分辨率档位。示例值：720。</param>
        /// <param name="Size">仅在使用 wan2.6 模型时返回。生成视频的分辨率。格式为“宽*高”，示例值：1920*1080。</param>
        /// <param name="VideoRatio">仅 wan2.5 及以下版本模型时返回。生成视频的分辨率。格式为“宽*高”，示例值：832*480。</param>
        /// <param name="VideoCount">生成视频的数量。固定为1。</param>
        internal record VideoSynthesisUsage(int? VideoDuration, float? Duration, int? InputVideoDuration, int? OutputVideoDuration, int? SR, string? Size, string? VideoRatio, int? VideoCount);
    }
}
