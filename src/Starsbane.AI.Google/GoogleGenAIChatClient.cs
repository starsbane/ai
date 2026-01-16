using Google.GenAI.Types;
using Microsoft.Extensions.AI;

namespace Starsbane.AI.Google
{
    public sealed class GoogleGenAIChatClient(GoogleAIClient parent) : BaseGoogleGenAISubClient(parent), IChatClient
    {
        private ChatClientMetadata _metadata => new(_providerName);

        /// <inheritdoc/>
        public async Task<ChatResponse> GetResponseAsync(IEnumerable<Microsoft.Extensions.AI.ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            Throw.IfNull(messages);
            Throw.IfLessThan(messages.Count(), 1);
            var mergedOptions = options.Merge(ClientOptions.Chat);
            _ = Throw.IfNullOrEmpty(mergedOptions.ModelId);

            return await PlatformClient.AsIChatClient(mergedOptions.ModelId).GetResponseAsync(messages, mergedOptions, cancellationToken);
        }

        /// <inheritdoc/>

        public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
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
        public object? GetService(System.Type serviceType, object? serviceKey = null)
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
