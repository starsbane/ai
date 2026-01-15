using System.Collections.ObjectModel;

namespace Starsbane.AI
{
    public class TextExtractionConfig : IPropertyEnvVariableMapping
    {
        public virtual ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping { get; }

        /// <summary>
        /// The desired language for result generation (a two-letter language code).
        /// </summary>
        public string? Language { get; set; }

        public static implicit operator TextExtractionOptions(TextExtractionConfig config) => To(config);
        public static implicit operator TextExtractionConfig(TextExtractionOptions options) => From(options);

        private static TextExtractionConfig From(TextExtractionOptions options)
        {
            Throw.IfNull(options);

            return new TextExtractionConfig
            {
                Language = options.Language
            };
        }

        private static TextExtractionOptions To(TextExtractionConfig config)
        {
            Throw.IfNull(config);

            return new TextExtractionOptions
            {
                Language = config.Language
            };
        }
    }
}
