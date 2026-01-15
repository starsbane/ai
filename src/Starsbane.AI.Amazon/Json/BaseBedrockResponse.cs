using Newtonsoft.Json;

namespace Starsbane.AI.Amazon.Json
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    internal abstract class BaseBedrockResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
