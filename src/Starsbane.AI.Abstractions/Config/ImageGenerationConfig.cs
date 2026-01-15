using Microsoft.Extensions.AI;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Starsbane.AI
{
    public class ImageGenerationConfig : IPropertyEnvVariableMapping
    {
        public virtual ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping { get; }

        /// <summary>
        /// Gets or sets the number of images to generate.
        /// </summary>
        public virtual int? Count { get; set; }

        /// <summary>
        /// Gets or sets the size of the generated image.
        /// </summary>
        /// <remarks>
        /// If a provider only supports fixed sizes, the closest supported size is used.
        /// </remarks>
        public virtual Size? ImageSize { get; set; }

        /// <summary>
        /// Gets or sets the media type (also known as MIME type) of the generated image.
        /// </summary>
        public virtual string? MediaType { get; set; }

        /// <summary>
        /// Gets or sets the model ID to use for image generation.
        /// </summary>
        public virtual string? ModelId { get; set; }

        /// <summary>
        /// Gets or sets the response format of the generated image.
        /// </summary>
        public virtual ImageGenerationResponseFormat? ResponseFormat { get; set; }

        /// <summary>
        /// Gets or sets the number of intermediate streaming images to generate.
        /// </summary>
        public virtual int? StreamingCount { get; set; }


        public static implicit operator ImageGenerationOptions(ImageGenerationConfig config) => To(config);
        public static implicit operator ImageGenerationConfig(ImageGenerationOptions options) => From(options);

        private static ImageGenerationConfig From(ImageGenerationOptions options)
        {
            Throw.IfNull(options);

            return new ImageGenerationConfig
            {
                Count = options.Count,
                ImageSize = options.ImageSize,
                MediaType = options.MediaType,
                ModelId = options.ModelId,
                ResponseFormat = options.ResponseFormat,
                StreamingCount = options.StreamingCount,
            };
        }

        private static ImageGenerationOptions To(ImageGenerationConfig config)
        {
            Throw.IfNull(config);

            return new ImageGenerationOptions()
            {
                Count = config.Count,
                ImageSize = config.ImageSize,
                MediaType = config.MediaType,
                ModelId = config.ModelId,
                ResponseFormat = config.ResponseFormat,
                StreamingCount = config.StreamingCount,
            };
        }
    }
}
