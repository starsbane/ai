using Google.Apis.Auth.OAuth2;
using Google.Cloud.DocumentAI.V1;
using Google.Protobuf;

namespace Starsbane.AI.Google
{
    public sealed class GoogleDocumentAIClient : BaseGoogleSubClient<DocumentProcessorServiceClient>, IDoucmentExtractionClient
    {
        public GoogleDocumentAIClient(GoogleAIClient parent) : base(parent)
        {
            var builder = new DocumentProcessorServiceClientBuilder();
            if (!string.IsNullOrEmpty(ClientOptions.GoogleApplicationCredentials))
            {
                var credential = GoogleCredential.FromFile(ClientOptions.GoogleApplicationCredentials);
                if (credential.IsCreateScopedRequired)
                {
                    credential = credential.CreateScoped("https://www.googleapis.com/auth/cloud-platform");
                }

                builder.Credential = credential;
            }
            else
            {
                builder.Credential = GoogleCredential.GetApplicationDefault();
            }
            PlatformClient = builder.Build();
        }

        /// <inheritdoc/>
        public async Task<string> ExtractFromDocument(BinaryData documentData, DocumentExtractionOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            Throw.IfNull(documentData);
            Throw.IfNullOrEmpty(documentData.MediaType);
            Throw.IfNullOrEmpty(ClientOptions.DocumentExtraction?.LocationID);
            Throw.IfNullOrEmpty(ClientOptions.DocumentExtraction?.ProcessorID);
            Throw.IfNullOrEmpty(ClientOptions.DocumentExtraction?.ProjectID);
            Throw.IfLessThan(documentData.Length, 1);

            var rawDocument = new RawDocument
            {
                Content = ByteString.CopyFrom(documentData),
                MimeType = documentData.MediaType
            };

            // Initialize request argument(s)
            var request = new ProcessRequest
            {
                Name = ProcessorName.FormatProjectLocationProcessor(
                    ClientOptions.DocumentExtraction?.ProjectID,
                    ClientOptions.DocumentExtraction?.LocationID,
                    ClientOptions.DocumentExtraction?.ProcessorID),
                RawDocument = rawDocument
            };

            // Make the request
            var response = await PlatformClient.ProcessDocumentAsync(request);

            return response.Document.Text;
        }
    }
}
