using Microsoft.Extensions.AI;
using OpenAI.Audio;


namespace Starsbane.AI.Azure
{
    public class AzureOpenAISpeechGenerator(AzureAIClient parent) : BaseAzureOpenAISubClient(parent), ISpeechGenerator
    {
        private readonly SpeechGeneratorMetadata _metadata = new(_providerName);

        public async Task<SpeechGenerationResponse> GenerateAsync(SpeechGenerationRequest request, SpeechGenerationOptions? options = null, CancellationToken cancellationToken = default)
        {
            Throw.IfNull(request);
            Throw.IfNullOrEmpty(request.Prompt);
            var mergedOptions = options.Merge(ClientOptions.Speech);
            Throw.IfNullOrEmpty(ClientOptions.Speech.DeploymentName);
            var deploymentName = Throw.IfNullOrEmpty(mergedOptions?.ModelId ?? ClientOptions.Speech?.DeploymentName).ToLower();
            var voiceId = Throw.IfNullOrEmpty(mergedOptions?.VoiceId).ToLower();
            var generatedSpeechVoice = new GeneratedSpeechVoice(voiceId);

            var speechGenerationOptions = new OpenAI.Audio.SpeechGenerationOptions()
            {
                ResponseFormat = GeneratedSpeechFormat.Mp3,
            };

            var audioClient = PlatformClient.GetAudioClient(deploymentName);
            var result = await audioClient.GenerateSpeechAsync(request.Prompt, generatedSpeechVoice, speechGenerationOptions, cancellationToken);
            return new SpeechGenerationResponse(new List<AIContent>() { new DataContent(new ReadOnlyMemory<byte>(result.Value.ToArray()), "audio/mp3") });
        }

        /// <inheritdoc/>
        public object? GetService(Type serviceType, object? serviceKey = null)
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
