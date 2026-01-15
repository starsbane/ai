using System.Diagnostics.CodeAnalysis;

namespace Starsbane.AI
{
    public class VideoGenerationRequest
    {
        /// <summary>Initializes a new instance of the <see cref="VideoGenerationRequest"/> class.</summary>
        public VideoGenerationRequest()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="VideoGenerationRequest"/> class.</summary>
        /// <param name="prompt">The prompt to guide the video generation.</param>
        public VideoGenerationRequest(string prompt)
        {
            Prompt = prompt;
        }

        /// <summary>Gets or sets the prompt to guide the video generation.</summary>
        public string? Prompt { get; set; }
    }
}
