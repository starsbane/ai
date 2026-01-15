using System.Collections.ObjectModel;

namespace Starsbane.AI.Google
{
    public class GoogleAIClientOptions : BaseAIConfig<GoogleChatOptionConfig, GoogleEmbeddingGenerationConfig, GoogleImageGenerationConfig,
        GoogleVideoGenerationConfig, GoogleSpeechGenerationConfig, GoogleTextExtractionConfig,
        GoogleDocumentExtractionConfig>
    {
        public const string SectionName = nameof(GoogleAIClientOptions);
        public bool IsVertex => string.IsNullOrEmpty(GeminiApiKey ?? GoogleApiKey);
        public string? GeminiApiKey { get; set; }
        public string? GoogleApiKey { get; set; }
        public string? VertexAIProjectName { get; set; }
        public string? VertexAILocation { get; set; }
        public string? GoogleApplicationCredentials { get; set; }

        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(GeminiApiKey), ["GEMINI_API_KEY"]},
                {nameof(GoogleApiKey), ["GOOGLE_API_KEY"]},
                {nameof(VertexAIProjectName), ["GOOGLE_PROJECT_ID", "GOOGLE_CLOUD_PROJECT"]},
                {nameof(VertexAILocation), ["GOOGLE_REGION", "GOOGLE_CLOUD_LOCATION"]},
                {nameof(GoogleApplicationCredentials), ["GOOGLE_APPLICATION_CREDENTIALS", "GOOGLE_WEB_CREDENTIALS"]},
            });
    }
    public sealed class GoogleEmbeddingGenerationConfig : EmbeddingGenerationConfig
    {
        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["GOOGLE_EMBEDDING_MODEL_ID"]},
            });
    }

    public sealed class GoogleChatOptionConfig : ChatOptionConfig
    {
        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["GOOGLE_CHAT_MODEL_ID"]},
            });
    }

    public sealed class GoogleImageGenerationConfig : ImageGenerationConfig
    {
        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["GOOGLE_IMAGE_MODEL_ID"]},
            });
    }

    public sealed class GoogleSpeechGenerationConfig : SpeechGenerationConfig
    {
        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["GOOGLE_SPEECH_MODEL_ID"]},
            });
    }

    public sealed class GoogleVideoGenerationConfig : VideoGenerationConfig
    {
        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["GOOGLE_VIDEO_MODEL_ID"]},
            });
    }

    public sealed class GoogleTextExtractionConfig : TextExtractionConfig
    {
    }

    public sealed class GoogleDocumentExtractionConfig : DocumentExtractionConfig
    {
        public string? ProjectID { get; set; }
        public string? LocationID { get; set; }
        public string? ProcessorID { get; set; }

        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ProjectID), ["GOOGLE_DOCUMENT_AI_PROJECT_ID"]},
                {nameof(LocationID), ["GOOGLE_DOCUMENT_AI_LOCATION_ID"]},
                {nameof(ProcessorID), ["GOOGLE_DOCUMENT_AI_PROCESSOR_ID"]}
            });
    }
}
