using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
using Image = Google.Cloud.Vision.V1.Image;

namespace Starsbane.AI.Google
{
    public sealed class GoogleImageAnnotatorClient : BaseGoogleSubClient<ImageAnnotatorClient>, IOCRClient
    {
        /// <inheritdoc/>
        public GoogleImageAnnotatorClient(GoogleAIClient parent) : base(parent)
        {
            var builder = new ImageAnnotatorClientBuilder();
            if (!string.IsNullOrEmpty(ClientOptions.GoogleApiKey))
            {
                builder.ApiKey = ClientOptions.GoogleApiKey;
            }
            else if (!string.IsNullOrEmpty(ClientOptions.GoogleApplicationCredentials))
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
        public async Task<string> ExtractFromImage(BinaryData imageData, TextExtractionOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            Throw.IfNull(imageData);
            Throw.IfLessThan((int)imageData.Length, 1);

            using var memoryStream = new MemoryStream(imageData.ToArray());
            cancellationToken.ThrowIfCancellationRequested();
            var image = await Image.FromStreamAsync(memoryStream);
            var annotation = await PlatformClient.DetectDocumentTextAsync(image);
            return annotation != null ? annotation.Text : "";
        }
    }
}
