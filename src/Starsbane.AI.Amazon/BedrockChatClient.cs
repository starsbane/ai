using Amazon.BedrockRuntime;
using Microsoft.Extensions.AI;

namespace Starsbane.AI.Amazon
{
    public sealed class BedrockChatClient(AmazonAIClient parent) : BaseBedrockRuntimeSubClient(parent), IChatClient
    {
        private readonly ChatClientMetadata _metadata = new(_providerName);

        /// <inheritdoc/>
        public async Task<ChatResponse> GetResponseAsync(IEnumerable<Microsoft.Extensions.AI.ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            Throw.IfNull(messages);
            Throw.IfLessThan(messages.Count(), 1);
            var mergedOptions = options.Merge(ClientOptions.Chat);
            _= Throw.IfNullOrEmpty(mergedOptions.ModelId);

            return await PlatformClient.AsIChatClient().GetResponseAsync(messages, mergedOptions, cancellationToken);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Throw.IfNull(messages);
            Throw.IfLessThan(messages.Count(), 1);
            var mergedOptions = options.Merge(ClientOptions.Chat);
            _ = Throw.IfNullOrEmpty(mergedOptions.ModelId);

            await foreach (var cru in PlatformClient.AsIChatClient().GetStreamingResponseAsync(messages, mergedOptions, cancellationToken))
            {
                yield return cru;
            }
        }

        /// <inheritdoc/>
        public object? GetService(Type serviceType, object? serviceKey = null)
        {
            Throw.IfNull(serviceType);

            return
                serviceKey is not null ? null :
                serviceType == typeof(ChatClientMetadata) ? _metadata :
                serviceType.IsInstanceOfType(PlatformClient) ? PlatformClient :
                serviceType.IsInstanceOfType(this) ? this :
                null;
        }
    }
}
