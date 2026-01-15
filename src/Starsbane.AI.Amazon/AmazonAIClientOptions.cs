using System.Collections.ObjectModel;
using Amazon.Extensions.NETCore.Setup;

namespace Starsbane.AI.Amazon
{
    public class AmazonAIClientOptions : BaseAIConfig<AmazonChatOptionConfig, AmazonEmbeddingGenerationConfig, AmazonImageGenerationConfig,
        AmazonVideoGenerationConfig, AmazonSpeechGenerationConfig, AmazonTextExtractionConfig,
        AmazonDocumentExtractionConfig>
    {
        public const string SectionName = nameof(AmazonAIClientOptions);
        public AWSOptions AWS { get; set; }
    }

    public sealed class AmazonEmbeddingGenerationConfig : EmbeddingGenerationConfig
    {
        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["AWS_EMBEDDING_MODEL_ID"]},
            });
    }

    public sealed class AmazonChatOptionConfig : ChatOptionConfig
    {
        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["AWS_CHAT_MODEL_ID"]},
            });
    }

    public sealed class AmazonImageGenerationConfig : ImageGenerationConfig
    {
        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["AWS_IMAGE_MODEL_ID"]},
            });
    }

    public sealed class AmazonSpeechGenerationConfig : SpeechGenerationConfig
    {
        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["AWS_SPEECH_MODEL_ID"]},
            });
    }

    public sealed class AmazonVideoGenerationConfig : VideoGenerationConfig
    {
        public Uri VideoOutputS3Uri { get; set; }

        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["AWS_VIDEO_MODEL_ID"]},
                {nameof(VideoOutputS3Uri), ["AWS_VIDEO_OUTPUT_S3_URI"]},
            });
    }

    public sealed class AmazonTextExtractionConfig : TextExtractionConfig
    {
    }

    public sealed class AmazonDocumentExtractionConfig : DocumentExtractionConfig
    {
    }
}
