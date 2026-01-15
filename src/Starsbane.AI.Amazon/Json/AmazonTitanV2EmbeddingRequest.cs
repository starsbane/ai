using Newtonsoft.Json;

namespace Starsbane.AI.Amazon.Json
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    internal class AmazonTitanV2EmbeddingRequest : BaseBedrockRequest
    {
        [JsonIgnore]
        public override string? TaskType { get; set; } = null;

        [JsonProperty("inputText")]
        public string InputText { get; set; }

        [JsonProperty("dimensions")]
        public int? Dimensions { get; set; }

        [JsonProperty("normalize")]
        public bool? Normalize { get; set; }

        [JsonProperty("embeddingTypes")]
        public IEnumerable<string>? EmbeddingTypes { get; set; }
    }
}
