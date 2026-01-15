using Cnblogs.DashScope.Core;
using Microsoft.Extensions.AI;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;
using ChatResponse = Microsoft.Extensions.AI.ChatResponse;

namespace Starsbane.AI.Alicloud
{
    public sealed class DashScopeChatClient(AlicloudAIClient parent) : BaseDashScopeSubClient(parent), IChatClient
    {
        private readonly ChatClientMetadata _metadata = new(_providerName);

        /// <inheritdoc/>
        public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            Throw.IfNull(messages);
            Throw.IfLessThan(messages.Count(), 1);
            var mergedOptions = options.Merge(ClientOptions.Chat);
            mergedOptions.ModelId = Throw.IfNullOrEmpty(mergedOptions.ModelId).ToLower();

            using var client = new Cnblogs.DashScope.AI.DashScopeChatClient(PlatformClient, mergedOptions.ModelId);
            return await client.GetResponseAsync(messages, mergedOptions, cancellationToken);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            Throw.IfNull(messages);
            Throw.IfLessThan(messages.Count(), 1);
            var mergedOptions = options.Merge(ClientOptions.Chat);
            mergedOptions.ModelId = Throw.IfNullOrEmpty(mergedOptions.ModelId).ToLower();

            using var client = new Cnblogs.DashScope.AI.DashScopeChatClient(PlatformClient, mergedOptions.ModelId);
            await foreach (var cru in client.GetStreamingResponseAsync(messages, mergedOptions, cancellationToken))
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
                serviceType == typeof(DashScopeClient) ? PlatformClient :
                serviceType == typeof(ChatClientMetadata) ? _metadata :
                serviceType.IsInstanceOfType(this) ? this :
                null;
        }
    }
}
