using System.Diagnostics.CodeAnalysis;

namespace Starsbane.AI
{
    public class SpeechGeneratorMetadata
    {
        /// <summary>Initializes a new instance of the <see cref="SpeechGeneratorMetadata"/> class.</summary>
        /// <param name="providerName">
        /// The name of the speech generation provider, if applicable. Where possible, this should map to the
        /// appropriate name defined in the OpenTelemetry Semantic Conventions for Generative AI systems.
        /// </param>
        /// <param name="providerUri">The URL for accessing the speech generation provider, if applicable.</param>
        /// <param name="defaultModelId">The ID of the speech generation model used by default, if applicable.</param>
        public SpeechGeneratorMetadata(string? providerName = null, Uri? providerUri = null, string? defaultModelId = null)
        {
            DefaultModelId = defaultModelId;
            ProviderName = providerName;
            ProviderUri = providerUri;
        }

        /// <summary>Gets the name of the speech generation provider.</summary>
        /// <remarks>
        /// Where possible, this maps to the appropriate name defined in the
        /// OpenTelemetry Semantic Conventions for Generative AI systems.
        /// </remarks>
        public string? ProviderName { get; }

        /// <summary>Gets the URL for accessing the speech generation provider.</summary>
        public Uri? ProviderUri { get; }

        /// <summary>Gets the ID of the default model used by this speech generator.</summary>
        /// <remarks>
        /// This value can be <see langword="null"/> if no default model is set on the corresponding <see cref="ISpeechGenerator"/>.
        /// An individual request may override this value via <see cref="SpeechGenerationOptions.ModelId"/>.
        /// </remarks>
        public string? DefaultModelId { get; }
    }
}
