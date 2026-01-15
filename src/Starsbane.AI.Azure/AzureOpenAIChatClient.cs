using Microsoft.Extensions.AI;

namespace Starsbane.AI.Azure
{
    public sealed class AzureOpenAIChatClient(AzureAIClient parent) : BaseAzureOpenAISubClient(parent), IChatClient
    {
        private readonly ChatClientMetadata _metadata = new(_providerName);

        /// <inheritdoc/>
        public async Task<ChatResponse> GetResponseAsync(IEnumerable<Microsoft.Extensions.AI.ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            Throw.IfNull(messages);
            Throw.IfLessThan(messages.Count(), 1);
            var mergedOptions = options.Merge(ClientOptions.Chat);
            var deploymentName = Throw.IfNullOrEmpty(mergedOptions?.ModelId ?? ClientOptions.Chat?.DeploymentName).ToLower();

            var chatClient = PlatformClient.GetChatClient(deploymentName).AsIChatClient();
            var result = await chatClient.GetResponseAsync(messages, options, cancellationToken);
            return result;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<Microsoft.Extensions.AI.ChatMessage> messages, ChatOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Throw.IfNull(messages);
            Throw.IfLessThan(messages.Count(), 1);
            var mergedOptions = options.Merge(ClientOptions.Chat);
            var deploymentName = Throw.IfNullOrEmpty(mergedOptions?.ModelId ?? ClientOptions.Chat?.DeploymentName).ToLower();

            var chatClient = PlatformClient.GetChatClient(deploymentName).AsIChatClient();
            await foreach (var c in chatClient.GetStreamingResponseAsync(messages, options, cancellationToken))
            {
                yield return c;
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
