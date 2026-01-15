using System.Text;
using Azure;
using Azure.AI.Vision.ImageAnalysis;

namespace Starsbane.AI.Azure
{
    public sealed class AzureImageAnalysisClient : BaseSubClient<AzureAIClient, ImageAnalysisClient, AzureAIClientOptions>, IOCRClient
    {
        public AzureImageAnalysisClient(AzureAIClient parent) : base(parent)
        {
            Throw.IfNullOrEmpty(ClientOptions.AzureVisionEndpoint);

            if (ClientOptions.CredentialType == CredentialType.ApiKeyCredential)
            {
                Throw.IfNullOrEmpty(ClientOptions.AzureVisionApiKey);
                PlatformClient = new ImageAnalysisClient(new Uri(ClientOptions.AzureVisionEndpoint), new AzureKeyCredential(ClientOptions.AzureVisionApiKey));
            }
            else
            {
                PlatformClient = new ImageAnalysisClient(new Uri(ClientOptions.AzureVisionEndpoint), AzureAIClient.ToTokenCredential(ClientOptions.CredentialType));
            }
        }

        /// <inheritdoc/>
        public async Task<string> ExtractFromImage(BinaryData imageData, TextExtractionOptions? options = null, CancellationToken cancellationToken = new CancellationToken())
        {
            Throw.IfNull(imageData);
            Throw.IfLessThan(imageData.Length, 1);

            var imageAnalysisOptions = new ImageAnalysisOptions
            {
                Language = options?.Language
            };
            // Start the async process to recognize the text
            var result = await PlatformClient.AnalyzeAsync(imageData, VisualFeatures.Read, imageAnalysisOptions, cancellationToken);
            if (result == null)
                throw new ApplicationException($"{nameof(PlatformClient.AnalyzeAsync)} returned null.");

            cancellationToken.ThrowIfCancellationRequested();
            if (result.Value.Read.Blocks.Any())
            {
                var stringBuilder = new StringBuilder();
                foreach (var block in result.Value.Read.Blocks)
                {
                    foreach (var line in block.Lines)
                    {
                        stringBuilder.AppendLine(line.Text);
                    }
                }
                return stringBuilder.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}
