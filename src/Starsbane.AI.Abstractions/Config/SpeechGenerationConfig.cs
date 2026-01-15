using System.Collections.ObjectModel;

namespace Starsbane.AI
{
    public class SpeechGenerationConfig : IPropertyEnvVariableMapping
    {
        public virtual ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping { get; }

        /// <summary>
        /// Gets or sets the voice ID for the generated speech.
        /// </summary>
        public virtual string? VoiceId { get; set; }

        /// <summary>
        /// Gets or sets the model ID to use for speech generation.
        /// </summary>
        public virtual string? ModelId { get; set; }

        /// <summary>
        /// Gets or sets the response format of the generated speech.
        /// </summary>
        public virtual SpeechGenerationResponseFormat? ResponseFormat { get; set; }

        public static implicit operator SpeechGenerationOptions(SpeechGenerationConfig config) => To(config);
        public static implicit operator SpeechGenerationConfig(SpeechGenerationOptions options) => From(options);

        private static SpeechGenerationConfig From(SpeechGenerationOptions options)
        {
            Throw.IfNull(options);

            return new SpeechGenerationConfig
            {
                VoiceId = options.VoiceId,
                ModelId = options.ModelId,
                ResponseFormat = options.ResponseFormat
            };
        }

        private static SpeechGenerationOptions To(SpeechGenerationConfig config)
        {
            Throw.IfNull(config);

            return new SpeechGenerationOptions
            {
                VoiceId = config.VoiceId,
                ModelId = config.ModelId,
                ResponseFormat = config.ResponseFormat
            };
        }
    }
}
