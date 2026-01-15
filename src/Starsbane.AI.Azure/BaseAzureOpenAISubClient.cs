using Azure.AI.OpenAI;
using System.ClientModel;

namespace Starsbane.AI.Azure
{
    public abstract class BaseAzureOpenAISubClient : BaseSubClient<AzureAIClient, AzureOpenAIClient, AzureAIClientOptions>
    {
        protected static readonly string _providerName = "azure.openai";

        protected BaseAzureOpenAISubClient(AzureAIClient parent) : base(parent)
        {
            Throw.IfNullOrEmpty(ClientOptions.AzureOpenAIEndpoint);

            AzureOpenAIClientOptions azureOpenAiClientOptions = new(AzureOpenAIClientOptions.ServiceVersion.V2025_04_01_Preview);

            if (ClientOptions.CredentialType == CredentialType.ApiKeyCredential)
            {
                Throw.IfNullOrEmpty(ClientOptions.AzureOpenAIApiKey);

                PlatformClient = new AzureOpenAIClient(new Uri(ClientOptions.AzureOpenAIEndpoint), new ApiKeyCredential(ClientOptions.AzureOpenAIApiKey), azureOpenAiClientOptions);
            }
            else
            {
                PlatformClient = new AzureOpenAIClient(new Uri(ClientOptions.AzureOpenAIEndpoint)!, AzureAIClient.ToTokenCredential(ClientOptions.CredentialType), azureOpenAiClientOptions);
            }
        }

        public virtual void Dispose()
        {
            // nothing to dispose
        }
    }
}
