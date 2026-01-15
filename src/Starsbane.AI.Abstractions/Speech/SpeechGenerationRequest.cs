using System.Diagnostics.CodeAnalysis;

namespace Starsbane.AI
{
    public class SpeechGenerationRequest
    {
        /// <summary>Initializes a new instance of the <see cref="SpeechGenerationRequest"/> class.</summary>
        public SpeechGenerationRequest()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SpeechGenerationRequest"/> class.</summary>
        /// <param name="prompt">The prompt to guide the speech generation.</param>
        public SpeechGenerationRequest(string prompt)
        {
            Prompt = prompt;
        }

        /// <summary>Gets or sets the prompt to guide the speech generation.</summary>
        public string? Prompt { get; set; }
    }
}
