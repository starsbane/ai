using System.Collections.ObjectModel;

namespace Starsbane.AI.Alicloud
{
    public class AlicloudAIClientOptions : BaseAIConfig<AlicloudChatOptionConfig, AlicloudEmbeddingGenerationConfig, AlicloudImageGenerationConfig,
        AlicloudVideoGenerationConfig, AlicloudSpeechGenerationConfig, AlicloudTextExtractionConfig,
        AlicloudDocumentExtractionConfig>
    {
        public const string SectionName = nameof(AlicloudAIClientOptions);

        public string DashscopeApiKey { get; set; }

        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(DashscopeApiKey), ["DASHSCOPE_API_KEY"]}
            });
    }

    public sealed class AlicloudEmbeddingGenerationConfig : EmbeddingGenerationConfig
    {
        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["DASHSCOPE_EMBEDDING_MODEL_ID"]},
            });
    }

    public sealed class AlicloudChatOptionConfig : ChatOptionConfig
    {
        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["DASHSCOPE_CHAT_MODEL_ID"]},
            });
    }

    public sealed class AlicloudImageGenerationConfig : ImageGenerationConfig
    {
        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["DASHSCOPE_IMAGE_MODEL_ID"]},
            });
    }

    public sealed class AlicloudSpeechGenerationConfig : SpeechGenerationConfig
    {
        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["DASHSCOPE_SPEECH_MODEL_ID"]},
            });
    }

    public sealed class AlicloudVideoGenerationConfig : VideoGenerationConfig
    {
        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["DASHSCOPE_VIDEO_MODEL_ID"]},
            });
    }

    public sealed class AlicloudTextExtractionConfig : TextExtractionConfig
    {
    }

    public sealed class AlicloudDocumentExtractionConfig : DocumentExtractionConfig
    {
    }
}
