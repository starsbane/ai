using Newtonsoft.Json;

namespace Starsbane.AI.Amazon.Json
{
    internal class TextToImageResponse : BaseBedrockResponse
    {
        /// <summary>
        /// When successful, a list of Base64-encoded strings that represent each image that was generated is returned.
        /// </summary>
        [JsonProperty("images")]
        public List<string> Images { get; set; }

        /// <summary>
        /// When you specified that the mask image should be returned with the output, this is where it is returned.
        /// </summary>
        [JsonProperty("maskImage")]
        public string MaskImage { get; set; }
    }
}
