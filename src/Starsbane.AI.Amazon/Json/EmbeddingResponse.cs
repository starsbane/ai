using Newtonsoft.Json;

namespace Starsbane.AI.Amazon.Json
{
    internal class EmbeddingResponse : BaseBedrockResponse
    {
        [JsonProperty("embedding")]
        public List<float> Embedding { get; set; }
    }
}
