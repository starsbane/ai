using System.Collections.ObjectModel;
using System.Drawing;

namespace Starsbane.AI
{
    public class DocumentExtractionConfig : IPropertyEnvVariableMapping
    {
        public virtual ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping { get; }

        /// <summary>
        /// The desired language for result generation (a two-letter language code).
        /// </summary>
        public string? Language { get; set; }

        public static implicit operator DocumentExtractionOptions(DocumentExtractionConfig config) => To(config);
        public static implicit operator DocumentExtractionConfig(DocumentExtractionOptions options) => From(options);

        private static DocumentExtractionConfig From(DocumentExtractionOptions options)
        {
            Throw.IfNull(options);

            return new DocumentExtractionConfig
            {
                Language = options.Language
            };
        }

        private static DocumentExtractionOptions To(DocumentExtractionConfig config)
        {
            Throw.IfNull(config);

            return new DocumentExtractionOptions
            {
                Language = config.Language
            };
        }
    }
}
