using System.Collections.ObjectModel;
using Microsoft.Extensions.AI;

namespace Starsbane.AI
{
    public class ChatOptionConfig : IPropertyEnvVariableMapping
    {
        public virtual ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping { get; }

        public static implicit operator ChatOptions(ChatOptionConfig config) => To(config);
        public static implicit operator ChatOptionConfig(ChatOptions options) => From(options);

        private static ChatOptionConfig From(ChatOptions options)
        {
            Throw.IfNull(options);

            return new ChatOptionConfig
            {
                Instructions = options.Instructions,
                Temperature = options.Temperature,
                MaxOutputTokens = options.MaxOutputTokens,
                TopP = options.TopP,
                TopK = options.TopK,
                FrequencyPenalty = options.FrequencyPenalty,
                PresencePenalty = options.PresencePenalty,
                Seed = options.Seed,
                ResponseFormat = options.ResponseFormat,
                ModelId = options.ModelId,
                StopSequences = options.StopSequences
            };
        }

        private static ChatOptions To(ChatOptionConfig config)
        {
            Throw.IfNull(config);

            return new ChatOptions
            {
                Instructions = config.Instructions,
                Temperature = config.Temperature,
                MaxOutputTokens = config.MaxOutputTokens,
                TopP = config.TopP,
                TopK = config.TopK,
                FrequencyPenalty = config.FrequencyPenalty,
                PresencePenalty = config.PresencePenalty,
                Seed = config.Seed,
                ResponseFormat = config.ResponseFormat,
                ModelId = config.ModelId,
                StopSequences = config.StopSequences?.ToList()
            };
        }

        /// <summary>Gets or sets additional per-request instructions to be provided to the <see cref="IChatClient"/>.</summary>
        public string? Instructions { get; set; }

        /// <summary>Gets or sets the temperature for generating chat responses.</summary>
        /// <remarks>
        /// This value controls the randomness of predictions made by the model. Use a lower value to decrease randomness in the response.
        /// </remarks>
        public float? Temperature { get; set; }

        /// <summary>Gets or sets the maximum number of tokens in the generated chat response.</summary>
        public int? MaxOutputTokens { get; set; }

        /// <summary>Gets or sets the "nucleus sampling" factor (or "top p") for generating chat responses.</summary>
        /// <remarks>
        /// Nucleus sampling is an alternative to sampling with temperature where the model
        /// considers the results of the tokens with <see cref="TopP"/> probability mass.
        /// For example, 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// </remarks>
        public float? TopP { get; set; }

        /// <summary>
        /// Gets or sets the number of most probable tokens that the model considers when generating the next part of the text.
        /// </summary>
        /// <remarks>
        /// This property reduces the probability of generating nonsense. A higher value gives more diverse answers, while a lower value is more conservative.
        /// </remarks>
        public int? TopK { get; set; }

        /// <summary>
        /// Gets or sets the penalty for repeated tokens in chat responses proportional to how many times they've appeared.
        /// </summary>
        /// <remarks>
        /// You can modify this value to reduce the repetitiveness of generated tokens. The higher the value, the stronger a penalty
        /// is applied to previously present tokens, proportional to how many times they've already appeared in the prompt or prior generation.
        /// </remarks>
        public float? FrequencyPenalty { get; set; }

        /// <summary>
        /// Gets or sets a value that influences the probability of generated tokens appearing based on their existing presence in generated text.
        /// </summary>
        /// <remarks>
        /// You can modify this value to reduce repetitiveness of generated tokens. Similar to <see cref="FrequencyPenalty"/>,
        /// except that this penalty is applied equally to all tokens that have already appeared, regardless of their exact frequencies.
        /// </remarks>
        public float? PresencePenalty { get; set; }

        /// <summary>Gets or sets a seed value used by a service to control the reproducibility of results.</summary>
        public long? Seed { get; set; }

        /// <summary>
        /// Gets or sets the response format for the chat request.
        /// </summary>
        /// <remarks>
        /// If <see langword="null"/>, no response format is specified and the client will use its default.
        /// This property can be set to <see cref="ChatResponseFormat.Text"/> to specify that the response should be unstructured text,
        /// to <see cref="ChatResponseFormat.Json"/> to specify that the response should be structured JSON data, or
        /// an instance of <see cref="ChatResponseFormatJson"/> constructed with a specific JSON schema to request that the
        /// response be structured JSON data according to that schema. It is up to the client implementation if or how
        /// to honor the request. If the client implementation doesn't recognize the specific kind of <see cref="ChatResponseFormat"/>,
        /// it can be ignored.
        /// </remarks>
        public ChatResponseFormat? ResponseFormat { get; set; }

        /// <summary>Gets or sets the model ID for the chat request.</summary>
        public string? ModelId { get; set; }

        /// <summary>
        /// Gets or sets the list of stop sequences.
        /// </summary>
        /// <remarks>
        /// After a stop sequence is detected, the model stops generating further tokens for chat responses.
        /// </remarks>
        public IList<string>? StopSequences { get; set; }
    }

    public class AzureChatConfig : ChatOptionConfig
    {

    }
}
