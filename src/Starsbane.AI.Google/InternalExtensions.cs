using Google.GenAI.Types;
using Microsoft.Extensions.AI;

namespace Starsbane.AI.Google
{
    internal static class InternalExtensions
    {
        internal static GenerateContentConfig AsGenerateContentConfig(this ChatOptions options)
        {

            return new GenerateContentConfig
            {
                SystemInstruction = (options?.Instructions == null) ? null : new Content
                {
                    Role = "System",
                    Parts =
                        [new Part { Text = options?.Instructions }]
                },
                Temperature = options?.Temperature,
                TopP = options?.TopP,
                TopK = options?.TopK,
                MaxOutputTokens = options?.MaxOutputTokens,
                StopSequences = options?.StopSequences?.ToList(),
                Seed = (int?)(options?.Seed),
                ResponseMimeType = (options != null)
                    ? (options?.ResponseFormat == ChatResponseFormat.Json) ? "application/json" : "text/plain"
                    : null
            };
        }

        internal static ChatFinishReason? AsChatFinishReason(this FinishReason? reason)
        {
            if (reason == FinishReason.PROHIBITED_CONTENT)
                return ChatFinishReason.ContentFilter;
            if (reason == FinishReason.STOP)
                return ChatFinishReason.Stop;
            if (reason == FinishReason.UNEXPECTED_TOOL_CALL)
                return ChatFinishReason.ToolCalls;
            if (reason == FinishReason.MAX_TOKENS)
                return ChatFinishReason.Length;
            return null;
        }
    }
}
