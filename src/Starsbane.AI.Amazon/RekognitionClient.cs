using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System.Text;

namespace Starsbane.AI.Amazon
{
    public sealed class RekognitionClient : BaseSubClient<AmazonAIClient, IAmazonRekognition, AmazonAIClientOptions>, IOCRClient
    {
        public RekognitionClient(AmazonAIClient parent) : base(parent) => PlatformClient = Parent.AWSOptions.CreateServiceClient<IAmazonRekognition>();

        /// <inheritdoc/>
        public async Task<string> ExtractFromImage(BinaryData imageData, TextExtractionOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Throw.IfNull(imageData);
            Throw.IfLessThan(imageData.Length, 1);
            var mergedOptions = options.Merge(ClientOptions.TextExtraction);

            using var memoryStream = new MemoryStream(imageData.ToArray());
            var detectTextRequest = new DetectTextRequest()
            {
                Image = new Image
                {
                    Bytes = memoryStream,
                }
            };

            var detectTextResponse = await PlatformClient.DetectTextAsync(detectTextRequest, cancellationToken);
            var stringBuilder = new StringBuilder();
            foreach (var detection in detectTextResponse.TextDetections.Where(d => d.Type == TextTypes.WORD))
            {
                stringBuilder.AppendLine(detection.DetectedText);
            }

            return stringBuilder.ToString();
        }
    }
}
