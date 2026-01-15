using Microsoft.Extensions.AI;
using System.Collections.ObjectModel;

namespace Starsbane.AI
{
    public class EmbeddingGenerationConfig : IPropertyEnvVariableMapping
    {
        public virtual ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping { get; }

        private int? _dimensions;
        /// <summary>Gets or sets the number of dimensions requested in the embedding.</summary>

        public virtual int? Dimensions
        {
            get => _dimensions;
            set
            {
                if (value is not null)
                {
                    _ = Throw.IfLessThan(value.Value, 1);
                }

                _dimensions = value;
            }
        }

        private int? _tokenLimit;
        /// <summary>Gets or sets the number of maximum token for each embedding string.</summary>

        public virtual int? TokenLimit
        {
            get => _tokenLimit;
            set
            {
                if (value is not null)
                {
                    _ = Throw.IfLessThan(value.Value, 1);
                }

                _tokenLimit = value;
            }
        }

        /// <summary>Gets or sets the model ID for the embedding generation request.</summary>
        public virtual string? ModelId { get; set; }

        public static implicit operator EmbeddingGenerationOptions(EmbeddingGenerationConfig config) => To(config);
        public static implicit operator EmbeddingGenerationConfig(EmbeddingGenerationOptions options) => From(options);

        private static EmbeddingGenerationConfig From(EmbeddingGenerationOptions options)
        {
            Throw.IfNull(options);

            return new EmbeddingGenerationConfig()
            {
                Dimensions = options.Dimensions,
                ModelId = options.ModelId,
            };
        }

        private static EmbeddingGenerationOptions To(EmbeddingGenerationConfig config)
        {
            Throw.IfNull(config);

            return new EmbeddingGenerationOptions()
            {
                Dimensions = config.Dimensions,
                ModelId = config.ModelId,
            };
        }
    }
}
