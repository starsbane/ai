using Microsoft.Extensions.AI;
using System.Text.Json.Serialization;

namespace Starsbane.AI
{
    public class DocumentExtractionOptions
    {

        /// <summary>Initializes a new instance of the <see cref="DocumentExtractionOptions"/> class.</summary>
        public DocumentExtractionOptions()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="DocumentExtractionOptions"/> class, performing a shallow copy of all properties from <paramref name="other"/>.</summary>
        protected DocumentExtractionOptions(DocumentExtractionOptions? other)
        {
            if (other is null)
            {
                return;
            }
            Language = other.Language;
            AdditionalProperties = other.AdditionalProperties?.Clone();
            RawRepresentationFactory = other.RawRepresentationFactory;
        }

        /// <summary>
        /// The desired language for result generation (a two-letter language code).
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// Gets or sets a callback responsible for creating the raw representation of the image generation options from an underlying implementation.
        /// </summary>
        /// <remarks>
        /// The underlying <see cref="IImageGenerator" /> implementation can have its own representation of options.
        /// When <see cref="IImageGenerator.GenerateAsync" /> is invoked with an <see cref="ImageGenerationOptions" />,
        /// that implementation can convert the provided options into its own representation in order to use it while performing
        /// the operation. For situations where a consumer knows  which concrete <see cref="IImageGenerator" /> is being used
        /// and how it represents options, a new instance of that implementation-specific options type can be returned by this
        /// callback for the <see cref="IImageGenerator" /> implementation to use instead of creating a new instance.
        /// Such implementations might mutate the supplied options instance further based on other settings supplied on this
        /// <see cref="ImageGenerationOptions" /> instance or from other inputs, therefore, it is <b>strongly recommended</b> to not
        /// return shared instances and instead make the callback return a new instance on each call.
        /// This is typically used to set an implementation-specific setting that isn't otherwise exposed from the strongly typed
        /// properties on <see cref="ImageGenerationOptions" />.
        /// </remarks>
        [JsonIgnore]
        public Func<IImageGenerator, object?>? RawRepresentationFactory { get; set; }

        /// <summary>Gets or sets any additional properties associated with the options.</summary>
        public AdditionalPropertiesDictionary? AdditionalProperties { get; set; }

        /// <summary>Produces a clone of the current <see cref="DocumentExtractionOptions"/> instance.</summary>
        /// <returns>A clone of the current <see cref="DocumentExtractionOptions"/> instance.</returns>
        public virtual DocumentExtractionOptions Clone() => new(this);
    }
}
