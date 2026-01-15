using System.Collections.ObjectModel;
using System.Drawing;

namespace Starsbane.AI
{
    public class VideoGenerationConfig : IPropertyEnvVariableMapping
    {
        public virtual ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping { get; }

        /// <summary>
        /// Gets or sets the number of frames per seconds of the generated video.
        /// </summary>
        public virtual int? FPS { get; set; }

        /// <summary>
        /// Gets or sets the duration of the generated video in seconds.
        /// </summary>
        public virtual int? Seconds { get; set; }

        /// <summary>
        /// Gets or sets the size of the generated video.
        /// </summary>
        /// <remarks>
        /// If a provider only supports fixed sizes, the closest supported size is used.
        /// </remarks>
        public virtual Size? VideoDimensions { get; set; }

        /// <summary>
        /// Gets or sets the model ID to use for video generation.
        /// </summary>
        public virtual string? ModelId { get; set; }

        /// <summary>
        /// Gets or sets the response format of the generated video.
        /// </summary>
        public virtual VideoGenerationResponseFormat? ResponseFormat { get; set; }

        /// <summary>
        /// Gets or sets the number of intermediate streaming videos to generate.
        /// </summary>
        public virtual int? StreamingCount { get; set; }

        public static implicit operator VideoGenerationOptions(VideoGenerationConfig config) => To(config);
        public static implicit operator VideoGenerationConfig(VideoGenerationOptions options) => From(options);

        private static VideoGenerationConfig From(VideoGenerationOptions options)
        {
            Throw.IfNull(options);

            return new VideoGenerationConfig
            {
                FPS = options.FPS,
                Seconds = options.Seconds,
                VideoDimensions = options.VideoDimensions,
                ModelId = options.ModelId,
                ResponseFormat = options.ResponseFormat,
                StreamingCount = options.StreamingCount,
            };
        }

        private static VideoGenerationOptions To(VideoGenerationConfig config)
        {
            Throw.IfNull(config);

            return new VideoGenerationOptions
            {
                FPS = config.FPS,
                Seconds = config.Seconds,
                VideoDimensions = config.VideoDimensions,
                ModelId = config.ModelId,
                ResponseFormat = config.ResponseFormat,
                StreamingCount = config.StreamingCount,
            };
        }
    }
}
