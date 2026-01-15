using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace Starsbane.AI.Azure
{
    // ReSharper disable once InconsistentNaming
    public class AzureAIClient : BaseGenAIClient<AzureAIClientOptions>, IAIClient<AzureAIClientOptions>
    {
        // Private constructors
        public AzureAIClient(AzureAIClientOptions clientOptions)
        {
            ClientOptions = clientOptions;
            AIClientExtensions.SetPropertiesInObject(ref _clientOptions);
        }

        public AzureAIClient(IOptions<AzureAIClientOptions> options) : this(options.Value)
        {
        }

        public Platform Platform { get; } = Platform.Azure;
        public IChatClient GetChatClient()
        {
            return new AzureOpenAIChatClient(this);
        }

        public IAIEmbeddingGenerator<string, Embedding<float>> GetEmbeddingGenerator()
        {
            return new AzureOpenAIEmbeddingGenerator(this);
        }

        public IVideoGenerator GetVideoGenerator()
        {
            return new SoraVideoGenerator(this);
        }

        public ISpeechGenerator GetSpeechGenerator()
        {
            return new AzureOpenAISpeechGenerator(this);
        }

        public IImageGenerator GetImageGenerator()
        {
            return new AzureOpenAIImageGenerator(this);
        }

        public IOCRClient GetOCRClient()
        {
            return new AzureImageAnalysisClient(this);
        }

        public IDoucmentExtractionClient GetDocumentExtractionClient()
        {
            return new AzureAIDocumentIntelligenceClient(this);
        }

        internal static TokenCredential ToTokenCredential(CredentialType credentialType)
        {
            return credentialType switch
            {
                CredentialType.DefaultAzureCredential => new DefaultAzureCredential(),
                CredentialType.AzureCliCredential => new AzureCliCredential(),
                CredentialType.AzureDeveloperCliCredential => new AzureDeveloperCliCredential(),
                CredentialType.AzurePowerShellCredential => new AzurePowerShellCredential(),
                CredentialType.DeviceCodeCredential => new DeviceCodeCredential(),
                CredentialType.EnvironmentCredential => new EnvironmentCredential(),
                CredentialType.InteractiveBrowserCredential => new InteractiveBrowserCredential(),
                CredentialType.ManagedIdentityCredential => new ManagedIdentityCredential(),
                CredentialType.VisualStudioCredential => new VisualStudioCredential(),
                CredentialType.WorkloadIdentityCredential => new WorkloadIdentityCredential(),
                _ => throw new InvalidOperationException("Unsupported credential type.")
            };
        }
    }
}
