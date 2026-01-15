using Amazon.Polly;
using Amazon.Polly.Model;
using Microsoft.Extensions.AI;


namespace Starsbane.AI.Amazon
{
    public sealed class PollySpeechGenerator : BaseSubClient<AmazonAIClient, IAmazonPolly, AmazonAIClientOptions>, ISpeechGenerator
    {
        private readonly SpeechGeneratorMetadata _metadata = new("aws.polly");

        public PollySpeechGenerator(AmazonAIClient parent) : base(parent) => PlatformClient = Parent.AWSOptions.CreateServiceClient<IAmazonPolly>();

        public async Task<SpeechGenerationResponse> GenerateAsync(SpeechGenerationRequest request, SpeechGenerationOptions? options = null, CancellationToken cancellationToken = default)
        {
            Throw.IfNull(request);
            Throw.IfNullOrEmpty(request.Prompt);
            var mergedOptions = options.Merge(ClientOptions.Speech);
            mergedOptions.VoiceId = Throw.IfNullOrEmpty(mergedOptions?.VoiceId);

            var synthesizeSpeechRequest = new SynthesizeSpeechRequest()
            {
                Text = request.Prompt,
                OutputFormat = OutputFormat.Mp3,
                VoiceId = VoiceId.FindValue(mergedOptions.VoiceId),
                Engine = Engine.Standard
            };

            var response = await PlatformClient.SynthesizeSpeechAsync(synthesizeSpeechRequest, cancellationToken);
            using var memoryStream = new MemoryStream();
            await response.AudioStream.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            return new SpeechGenerationResponse(new List<AIContent>() { new DataContent(new ReadOnlyMemory<byte>(bytes), "audio/mp3") });
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

        /// <inheritdoc/>
        public void Dispose()
        {
            PlatformClient.Dispose();
        }
    }
}
