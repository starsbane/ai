using Newtonsoft.Json;

namespace Starsbane.AI.Amazon.Json
{
    internal class AmazonNovaEmbeddingRequest : BaseBedrockRequest
    {
        internal class singleEmbeddingParams
        {
            internal class singleEmbeddingParamsText
            {
                [JsonProperty("truncationMode")]
                public string TruncationMode { get; set; }

                [JsonProperty("value")]
                public string Value { get; set; }

            }

            [JsonProperty("embeddingPurpose")]
            public string EmbeddingPurpose { get; set; }

            [JsonProperty("embeddingDimension")]
            public int EmbeddingDimension { get; set; }

            [JsonProperty("text")]
            public singleEmbeddingParamsText Text { get; } = new();
        }

        public override string? TaskType { get; set; } = "SINGLE_EMBEDDING";

        [JsonProperty("singleEmbeddingParams")]
        public singleEmbeddingParams SingleEmbeddingParams { get; } = new();
    }
}
