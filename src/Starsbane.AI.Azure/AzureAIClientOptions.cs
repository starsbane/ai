using System.Collections.ObjectModel;

namespace Starsbane.AI.Azure
{
    public class AzureAIClientOptions :
        BaseAIConfig<AzureChatOptionConfig, AzureEmbeddingGenerationConfig, AzureImageGenerationConfig,
            AzureVideoGenerationConfig, AzureSpeechGenerationConfig, AzureTextExtractionConfig,
            AzureDocumentExtractionConfig>
    {
        public const string SectionName = nameof(AzureAIClientOptions);
        public CredentialType CredentialType { get; set; } = CredentialType.ApiKeyCredential;

        public string? AzureOpenAIApiKey { get; set; }

        public string? AzureOpenAIEndpoint { get; set; }

        public string? AzureVisionApiKey { get; set; }

        public string? AzureVisionEndpoint { get; set; }

        public string? DocumentIntelligenceApiKey { get; set; }

        public string? DocumentIntelligenceEndpoint { get; set; }

        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(AzureOpenAIApiKey), ["AZURE_OPENAI_API_KEY"]},
                {nameof(AzureOpenAIEndpoint), ["AZURE_OPENAI_ENDPOINT"]},
                {nameof(AzureVisionApiKey), ["AZURE_VISION_API_KEY"]},
                {nameof(AzureVisionEndpoint), ["AZURE_VISION_ENDPOINT"]},
                {nameof(DocumentIntelligenceApiKey), ["AZURE_DOCUMENT_INTELLIGENCE_API_KEY"]},
                {nameof(DocumentIntelligenceEndpoint), ["AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT"]},
            });
    }

    public sealed class AzureEmbeddingGenerationConfig : EmbeddingGenerationConfig
    {
        public string DeploymentName { get; set; }
        public string? ApiKey { get; set; }
        public string? Endpoint { get; set; }

        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(DeploymentName), ["AZURE_OPENAI_EMBEDDING_DEPLOYMENT_NAME"]},
                {nameof(ApiKey), ["AZURE_OPENAI_EMBEDDING_API_KEY"]},
                {nameof(Endpoint), ["AZURE_OPENAI_EMBEDDING_ENDPOINT"]},
            });
    }

    public sealed class AzureChatOptionConfig : ChatOptionConfig
    {
        public string DeploymentName { get; set; }
        public string? ApiKey { get; set; }
        public string? Endpoint { get; set; }

        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(DeploymentName), ["AZURE_OPENAI_CHAT_DEPLOYMENT_NAME"]},
                {nameof(ApiKey), ["AZURE_OPENAI_CHAT_API_KEY"]},
                {nameof(Endpoint), ["AZURE_OPENAI_CHAT_ENDPOINT"]},
            });
    }

    public sealed class AzureImageGenerationConfig : ImageGenerationConfig
    {
        public string DeploymentName { get; set; }
        public string? ApiKey { get; set; }
        public string? Endpoint { get; set; }

        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(DeploymentName), ["AZURE_OPENAI_IMAGE_DEPLOYMENT_NAME"]},
                {nameof(ApiKey), ["AZURE_OPENAI_IMAGE_API_KEY"]},
                {nameof(Endpoint), ["AZURE_OPENAI_IMAGE_ENDPOINT"]},
            });
    }

    public sealed class AzureSpeechGenerationConfig : SpeechGenerationConfig
    {
        public string DeploymentName { get; set; }
        public string? ApiKey { get; set; }
        public string? Endpoint { get; set; }

        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(DeploymentName), ["AZURE_OPENAI_SPEECH_DEPLOYMENT_NAME"]},
                {nameof(ApiKey), ["AZURE_OPENAI_EMBEDDING_API_KEY"]},
                {nameof(Endpoint), ["AZURE_OPENAI_EMBEDDING_ENDPOINT"]},
            });
    }

    public sealed class AzureVideoGenerationConfig : VideoGenerationConfig
    {
        public string DeploymentName { get; set; }

        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(DeploymentName), ["AZURE_OPENAI_VIDEO_DEPLOYMENT_NAME"]},
            });
    }

    public sealed class AzureTextExtractionConfig : TextExtractionConfig
    {
    }

    public sealed class AzureDocumentExtractionConfig : DocumentExtractionConfig
    {
        public string ModelId { get; set; } = "prebuilt-read";

        public override ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping => new(
            new Dictionary<string, string[]>()
            {
                {nameof(ModelId), ["AZURE_DOCUMENT_EXTRACTION_MODEL_ID"]},
            });
    }
}
