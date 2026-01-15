using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Newtonsoft.Json;
using Starsbane.AI.Amazon.Json;
using System.Text;

namespace Starsbane.AI.Amazon
{
    public abstract class BaseBedrockRuntimeSubClient(AmazonAIClient parent) : BaseAmazonSubClient<IAmazonBedrockRuntime>(parent)
    {
        protected static readonly string _providerName = "aws.bedrock";

        internal async Task<InvokeModelResponse> InvokeModel<TRequest>(string modelId, TRequest request, string acceptType, CancellationToken cancellationToken) where TRequest : BaseBedrockRequest
        {
            var jsonPayload = JsonConvert.SerializeObject(request);
            var jsonPayloadBytes = Encoding.UTF8.GetBytes(jsonPayload);

            var modelRequest = new InvokeModelRequest()
            {
                ModelId = modelId,
                Body = new MemoryStream(jsonPayloadBytes),
                Accept = acceptType,
                ContentType = "application/json"
            };

            return await PlatformClient.InvokeModelAsync(modelRequest, cancellationToken);
        }
    }
}