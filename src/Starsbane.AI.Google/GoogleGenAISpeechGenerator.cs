using Google.GenAI.Types;
using Microsoft.Extensions.AI;


namespace Starsbane.AI.Google
{
    public sealed class GoogleGenAISpeechGeneratorr(GoogleAIClient parent, ISpeechFormatConverter speechFormatConverter) : BaseGoogleGenAISubClient(parent), ISpeechGenerator
    {
        private SpeechGeneratorMetadata _metadata => new(_providerName);

        /// <inheritdoc/>
        public async Task<SpeechGenerationResponse> GenerateAsync(SpeechGenerationRequest request, SpeechGenerationOptions? options = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Throw.IfNull(request);
            Throw.IfNullOrEmpty(request.Prompt);
            var mergedOptions = options.Merge(ClientOptions.Speech);
            mergedOptions.ModelId = Throw.IfNullOrEmpty(ClientOptions.Speech.ModelId).ToLower();
            mergedOptions.VoiceId = Throw.IfNullOrEmpty(ClientOptions.Speech.VoiceId).ToLower();

            List<Content> contents = [new() { Parts = [new Part() { Text = request.Prompt }] }];

            GenerateContentConfig config = new()
            {
                ResponseModalities = ["AUDIO"],
                SpeechConfig = new()
                {
                    VoiceConfig = new()
                    {
                        PrebuiltVoiceConfig = new() { VoiceName = mergedOptions.VoiceId }
                    }
                }
            };

            var operation = await PlatformClient.Models.GenerateContentAsync(mergedOptions.ModelId, contents, config);
            var speechBytes = operation.Candidates?.First()?.Content?.Parts?.First()?.InlineData?.Data;
            var bytes = await speechFormatConverter.ConvertSpeechFormatAsync(speechBytes!, cancellationToken);
            return new SpeechGenerationResponse(new List<AIContent>() { new DataContent(new ReadOnlyMemory<byte>(bytes), "audio/mp3") });
        }


        /// <inheritdoc/>
        public object? GetService(System.Type serviceType, object? serviceKey = null)
        {
            Throw.IfNull(serviceType);

            return
                serviceKey is not null ? null :
                serviceType == typeof(SpeechGeneratorMetadata) ? _metadata :
                serviceType.IsInstanceOfType(PlatformClient) ? PlatformClient :
                serviceType.IsInstanceOfType(this) ? this :
                null;
        }

    }
}
