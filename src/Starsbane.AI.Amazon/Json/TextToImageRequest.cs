using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Starsbane.AI.Amazon.Json
{
    internal class TextToImageRequest : BaseBedrockRequest
    {
        [JsonProperty("taskType")]
        public override string? TaskType { get; set; } = "TEXT_IMAGE";

        [JsonProperty("textToImageParams", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TextToImageParams TextToImageParams { get; set; }

        [JsonProperty("imageGenerationConfig", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ImageGenerationConfig ImageGenerationConfig { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    internal class ImageGenerationConfig
    {
        /// <summary>
        ///  (Optional) – Define the size and aspect ratio of the generated image. Default to 1024.
        /// </summary>
        [JsonProperty("width")]
        public int? Width { get; set; }

        /// <summary>
        ///  (Optional) – Define the size and aspect ratio of the generated image. Default to 1024.
        /// </summary>
        [JsonProperty("height")]
        public int? Height { get; set; }

        /// <summary>
        /// (Optional) - Specifies the quality to use when generating the image
        /// - "standard" (default) or "premium"
        /// </summary>
        [JsonProperty("quality")]
        public Quality? Quality { get; set; }

        /// <summary>
        /// (Optional) – Specifies how strictly the model should adhere to the prompt.
        /// Values range from 1.1-10, inclusive, and the default value is 6.5.
        /// </summary>
        [JsonProperty("cfgScale")]
        public float? CfgScale { get; set; }

        /// <summary>
        /// (Optional) – Determines the initial noise setting for the generation process.
        /// </summary>
        [JsonProperty("seed")]
        public int? Seed { get; set; }

        /// <summary>
        /// (Optional) – The number of images to generate.
        /// </summary>
        [JsonProperty("numberOfImages")]
        public int? NumberOfImages { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    internal class TextToImageParams
    {
        /// <summary>
        /// (Required) – A text prompt that describes what to generate within the masked region.
        /// The prompt must be 1-1024 characters in length.
        /// </summary>
        [JsonProperty("text")]
        [MinLength(1)]
        [MaxLength(1024)]
        [JsonRequired]
        public string Text { get; set; }

        /// <summary>
        ///  (Optional) – A text prompt to define what not to include in the image.
        /// This value must be 1-1024 characters in length.
        /// </summary>
        [JsonProperty("negativeText")]
        [MinLength(1)]
        [MaxLength(1024)]
        public string? NegativeText { get; set; }

        /// <summary>
        ///  (Optional) – Specifies the style that is used to generate this image.
        /// </summary>
        [JsonProperty("style")]
        public GenerateImageStyle? Style { get; set; }
    }
}
