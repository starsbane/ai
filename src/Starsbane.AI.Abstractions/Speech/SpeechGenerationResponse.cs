using Microsoft.Extensions.AI;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Starsbane.AI
{
    public class SpeechGenerationResponse
    {
        /// <summary>Initializes a new instance of the <see cref="SpeechGenerationResponse"/> class.</summary>
        [JsonConstructor]
        public SpeechGenerationResponse()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SpeechGenerationResponse"/> class.</summary>
        /// <param name="contents">The contents for this response.</param>
        public SpeechGenerationResponse(IList<AIContent>? contents)
        {
            Contents = contents;
        }

        /// <summary>Gets or sets the raw representation of the speech generation response from an underlying implementation.</summary>
        /// <remarks>
        /// If a <see cref="SpeechGenerationResponse"/> is created to represent some underlying object from another object
        /// model, this property can be used to store that original object. This can be useful for debugging or
        /// for enabling a consumer to access the underlying object model if needed.
        /// </remarks>
        [JsonIgnore]
        public object? RawRepresentation { get; set; }

        /// <summary>
        /// Gets or sets the generated content items.
        /// </summary>
        /// <remarks>
        /// Content is typically <see cref="DataContent"/> for speechs streamed from the generator, or <see cref="UriContent"/> for remotely hosted speechs, but
        /// can also be provider-specific content types that represent the generated speechs.
        /// </remarks>
        public IList<AIContent> Contents { get; set; }

        /// <summary>Gets or sets usage details for the speech generation response.</summary>
        public UsageDetails? Usage { get; set; }
    }
}
