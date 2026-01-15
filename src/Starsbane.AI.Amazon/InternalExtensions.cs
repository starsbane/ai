using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Microsoft.Extensions.AI;

namespace Starsbane.AI.Amazon
{
    internal static class BedrockRuntimeExtensions
    {
        internal static ConversationRole AsConversationRole(this ChatRole chatRole)
        {
            if (chatRole == ChatRole.Assistant)
                return ConversationRole.Assistant;
            else if (chatRole == ChatRole.User)
                return ConversationRole.User;
            else
                throw new ArgumentOutOfRangeException(nameof(chatRole), $"Invalid role: {chatRole.Value}");
        }

        internal static ChatRole AsChatRole(this ConversationRole conversationRole)
        {
            if (conversationRole == ConversationRole.Assistant)
                return ChatRole.Assistant;
            else if (conversationRole == ConversationRole.User)
                return ChatRole.User;
            else
                throw new ArgumentOutOfRangeException(nameof(conversationRole),
                    $"Invalid role: {conversationRole.Value}");
        }

        internal static List<Message> AsConverseMessages(
            this IEnumerable<Microsoft.Extensions.AI.ChatMessage> messages)
        {
            return messages.Where(message => message.Role == ChatRole.Assistant || message.Role == ChatRole.User).Select(message => new Message
            {
                Content = [new() { Text = message.Text }],
                Role = message.Role.AsConversationRole()
            }).ToList();
        }

        internal static List<SystemContentBlock> AsSystemContentBlock(
            this IEnumerable<Microsoft.Extensions.AI.ChatMessage> messages)
        {
            return messages.Where(message => message.Role == ChatRole.System).Select(message => new SystemContentBlock
            {
                Text = message.Text
            }).ToList();
        }

        internal static UsageDetails AsUsageDetails(this TokenUsage tokenUsage) =>
            new()
            {
                InputTokenCount = tokenUsage.InputTokens,
                OutputTokenCount = tokenUsage.OutputTokens,
                TotalTokenCount = tokenUsage.TotalTokens,
                AdditionalCounts = new(new Dictionary<string, long>
                {
                    {
                        nameof(tokenUsage.CacheReadInputTokens),
                        Convert.ToInt64(tokenUsage.CacheReadInputTokens)
                    },
                    {
                        nameof(tokenUsage.CacheWriteInputTokens),
                        Convert.ToInt64(tokenUsage.CacheWriteInputTokens)
                    },
                })
            };
    }
}
