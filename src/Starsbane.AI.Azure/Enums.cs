namespace Starsbane.AI.Azure
{
    public enum CredentialType
    {
        // Derived from Azure.Core.TokenCredential
        DefaultAzureCredential,
        //AuthorizationCodeCredential,
        AzureCliCredential,
        AzureDeveloperCliCredential,
        //AzurePipelinesCredential,
        AzurePowerShellCredential,
        //ChainedTokenCredential,
        //ClientAssertionCredential,
        //ClientCertificateCredential,
        //ClientSecretCredential,
        DeviceCodeCredential,
        EnvironmentCredential,
        InteractiveBrowserCredential,
        ManagedIdentityCredential,
        //OnBehalfOfCredential,
        VisualStudioCredential,
        WorkloadIdentityCredential,

        // Derived from System.ClientModel.ApiKeyCredential
        ApiKeyCredential
    }

    public enum SpeechVoice
    {
        Alloy,
        Echo,
        Fable,
        Onyx,
        Nova,
        Shimmer
    }
}
