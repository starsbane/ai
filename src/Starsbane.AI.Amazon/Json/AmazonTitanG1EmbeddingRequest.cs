using Newtonsoft.Json;

namespace Starsbane.AI.Amazon.Json
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    internal class AmazonTitanG1EmbeddingRequest : BaseBedrockRequest
    {
        [JsonIgnore]
        public override string? TaskType { get; set; } = null;

        [JsonProperty("inputText")]
        public string InputText { get; set; }
    }
}
