using Newtonsoft.Json;

namespace Starsbane.AI.Amazon.Json
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    internal abstract class BaseBedrockRequest
    {
        [JsonProperty("taskType")]
        public abstract string? TaskType { get; set; }
    }
}
