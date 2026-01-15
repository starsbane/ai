using Microsoft.Extensions.AI;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;



namespace Starsbane.AI
{
    public class SpeechGenerationOptions
    {
        /// <summary>Initializes a new instance of the <see cref="SpeechGenerationOptions"/> class.</summary>
        public SpeechGenerationOptions()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SpeechGenerationOptions"/> class, performing a shallow copy of all properties from <paramref name="other"/>.</summary>
        protected SpeechGenerationOptions(SpeechGenerationOptions? other)
        {
            if (other is null)
            {
                return;
            }

            AdditionalProperties = other.AdditionalProperties?.Clone();
            VoiceId = other.VoiceId;
            ModelId = other.ModelId;
            RawRepresentationFactory = other.RawRepresentationFactory;
            ResponseFormat = other.ResponseFormat;
        }

        /// <summary>
        /// Gets or sets the voice ID for the generated speech.
        /// </summary>
        public string? VoiceId { get; set; }

        /// <summary>
        /// Gets or sets the model ID to use for speech generation.
        /// </summary>
        public string? ModelId { get; set; }

        /// <summary>
        /// Gets or sets a callback responsible for creating the raw representation of the speech generation options from an underlying implementation.
        /// </summary>
        /// <remarks>
        /// The underlying <see cref="ISpeechGenerator" /> implementation can have its own representation of options.
        /// When <see cref="ISpeechGenerator.GenerateAsync" /> is invoked with an <see cref="SpeechGenerationOptions" />,
        /// that implementation can convert the provided options into its own representation in order to use it while performing
        /// the operation. For situations where a consumer knows  which concrete <see cref="ISpeechGenerator" /> is being used
        /// and how it represents options, a new instance of that implementation-specific options type can be returned by this
        /// callback for the <see cref="ISpeechGenerator" /> implementation to use instead of creating a new instance.
        /// Such implementations might mutate the supplied options instance further based on other settings supplied on this
        /// <see cref="SpeechGenerationOptions" /> instance or from other inputs, therefore, it is <b>strongly recommended</b> to not
        /// return shared instances and instead make the callback return a new instance on each call.
        /// This is typically used to set an implementation-specific setting that isn't otherwise exposed from the strongly typed
        /// properties on <see cref="SpeechGenerationOptions" />.
        /// </remarks>
        [JsonIgnore]
        public Func<ISpeechGenerator, object?>? RawRepresentationFactory { get; set; }

        /// <summary>
        /// Gets or sets the response format of the generated speech.
        /// </summary>
        public SpeechGenerationResponseFormat? ResponseFormat { get; set; }

        /// <summary>Gets or sets any additional properties associated with the options.</summary>
        public AdditionalPropertiesDictionary? AdditionalProperties { get; set; }

        /// <summary>Produces a clone of the current <see cref="SpeechGenerationOptions"/> instance.</summary>
        /// <returns>A clone of the current <see cref="SpeechGenerationOptions"/> instance.</returns>
        public virtual SpeechGenerationOptions Clone() => new(this);
    }

    /// <summary>
    /// Represents the requested response format of the generated image.
    /// </summary>
    /// <remarks>
    /// Not all implementations support all response formats and this value might be ignored by the implementation if not supported.
    /// </remarks>
    public enum SpeechGenerationResponseFormat
    {
        /// <summary>
        /// The generated image is returned as a URI pointing to the image resource.
        /// </summary>
        Uri,

        /// <summary>
        /// The generated image is returned as in-memory image data.
        /// </summary>
        Data,

        /// <summary>
        /// The generated image is returned as a hosted resource identifier, which can be used to retrieve the image later.
        /// </summary>
        Hosted,
    }
}
