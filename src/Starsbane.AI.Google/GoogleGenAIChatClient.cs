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

            var contents = messages.Select(m => new Content()
            {
                Role = m.Role.Value.ToLower(),
                Parts = [new Part() { Text = m.Text }],
            }).ToList();

            var response = await PlatformClient.Models.GenerateContentAsync(mergedOptions.ModelId, contents, options.AsGenerateContentConfig());

            var c = response.Candidates?.First();
            return new ChatResponse
            {
                Messages = new List<ChatMessage>()
                {
                    new()
                    {
                        Role = new ChatRole(c.Content.Role ?? "user"),
                        Contents = c.Content.Parts?.Select(p => new TextContent(p.Text) as AIContent).ToList(),
                    }
                },
                ResponseId = response.ResponseId,
                CreatedAt = response.CreateTime,
                Usage = new UsageDetails
                {
                    InputTokenCount = response.UsageMetadata?.PromptTokenCount,
                    OutputTokenCount = response.UsageMetadata?.CandidatesTokenCount,
                    TotalTokenCount = response.UsageMetadata?.TotalTokenCount,
                    CachedInputTokenCount = response.UsageMetadata?.CachedContentTokenCount,
                    ReasoningTokenCount = response.UsageMetadata?.ThoughtsTokenCount,
                },
                RawRepresentation = response.SdkHttpResponse,
                FinishReason = c.FinishReason.AsChatFinishReason(),
            };
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

            var contents = messages.Select(m => new Content()
            {
                Role = m.Role.Value.ToLower(),
                Parts = [new Part() { Text = m.Text }],
            }).ToList();

            await foreach (var response in PlatformClient.Models.GenerateContentStreamAsync(mergedOptions.ModelId, contents,
                               options.AsGenerateContentConfig()))
            {
                var c = response.Candidates?.First();
                yield return new ChatResponseUpdate
                {
                    Role = new ChatRole(c.Content.Role),
                    Contents = c.Content.Parts?.Select(p => new TextContent(p.Text) as AIContent).ToList(),
                    ResponseId = response.ResponseId,
                    CreatedAt = response.CreateTime,
                    FinishReason = c.FinishReason.AsChatFinishReason(),
                    RawRepresentation = response.SdkHttpResponse
                };
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
