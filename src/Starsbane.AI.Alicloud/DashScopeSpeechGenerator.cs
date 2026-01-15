using Cnblogs.DashScope.Core;
using Microsoft.Extensions.AI;



namespace Starsbane.AI.Alicloud
{
    public sealed class DashScopeSpeechGenerator(AlicloudAIClient parent) : BaseDashScopeSubClient(parent), ISpeechGenerator
    {
        private readonly SpeechGeneratorMetadata _metadata = new(_providerName);

        /// <inheritdoc/>
        public async Task<SpeechGenerationResponse> GenerateAsync(SpeechGenerationRequest request, SpeechGenerationOptions? options = null, CancellationToken cancellationToken = default)
        {
            Throw.IfNull(request);
            Throw.IfNullOrEmpty(request.Prompt);
            var mergedOptions = options.Merge(ClientOptions.Speech);
            mergedOptions.ModelId = Throw.IfNullOrEmpty(mergedOptions.ModelId);
            mergedOptions.VoiceId = Throw.IfNullOrEmpty(mergedOptions?.VoiceId ?? "longanyang");

            using var tts = await PlatformClient.CreateSpeechSynthesizerSocketSessionAsync(mergedOptions.ModelId, cancellationToken);
            var taskId = await tts.RunTaskAsync(new SpeechSynthesizerParameters { Voice = mergedOptions.VoiceId, Format = "mp3" }, request.Prompt, cancellationToken);
            await tts.FinishTaskAsync(taskId, cancellationToken);

            var memoryStream = new MemoryStream();
            await foreach (var b in tts.GetAudioAsync().WithCancellation(cancellationToken))
            {
                memoryStream.WriteByte(b);
            }

            if (memoryStream.Length > 0)
                return new SpeechGenerationResponse(new List<AIContent>() { new DataContent(new ReadOnlyMemory<byte>(memoryStream.ToArray()), "audio/mp3") });

            throw new InvalidOperationException($"Operation failed as the received audio length is zero.");
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
