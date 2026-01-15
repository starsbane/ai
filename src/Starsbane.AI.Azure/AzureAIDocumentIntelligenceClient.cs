using Azure;
using Azure.AI.DocumentIntelligence;

namespace Starsbane.AI.Azure
{
    public sealed class AzureAIDocumentIntelligenceClient : BaseSubClient<AzureAIClient, DocumentIntelligenceClient, AzureAIClientOptions>, IDoucmentExtractionClient
    {
        /// <inheritdoc/>
        public AzureAIDocumentIntelligenceClient(AzureAIClient parent) : base(parent)
        {
            Throw.IfNullOrEmpty(ClientOptions.DocumentIntelligenceEndpoint);

            if (ClientOptions.CredentialType == CredentialType.ApiKeyCredential)
            {
                Throw.IfNullOrEmpty(ClientOptions.DocumentIntelligenceApiKey);
                PlatformClient = new DocumentIntelligenceClient(new Uri(ClientOptions.DocumentIntelligenceEndpoint), new AzureKeyCredential(ClientOptions.DocumentIntelligenceApiKey));
            }
            else
            {
                PlatformClient = new DocumentIntelligenceClient(new Uri(ClientOptions.DocumentIntelligenceEndpoint), AzureAIClient.ToTokenCredential(ClientOptions.CredentialType));
            }
        }

        /// <inheritdoc/>
        public async Task<string> ExtractFromDocument(BinaryData documentData, DocumentExtractionOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Throw.IfNull(documentData);
            Throw.IfNullOrEmpty(documentData.MediaType);
            Throw.IfLessThan(documentData.Length, 1);
            Throw.IfNullOrEmpty(ClientOptions.DocumentExtraction.ModelId);

            var mergedOptions = options.Merge(ClientOptions.DocumentExtraction);

            var analyzeDocumentOptions = new AnalyzeDocumentOptions(ClientOptions.DocumentExtraction.ModelId, documentData)
            {
                Locale = options?.Language,
                OutputContentFormat = DocumentContentFormat.Markdown
            };

            var operation = await PlatformClient.AnalyzeDocumentAsync(WaitUntil.Completed, analyzeDocumentOptions, cancellationToken);
            var result = operation.Value;
            return result.Content;
        }
    }
}
