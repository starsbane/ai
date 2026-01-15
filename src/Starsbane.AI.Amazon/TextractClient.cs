using Amazon.Textract;
using Amazon.Textract.Model;
using System.Text;

namespace Starsbane.AI.Amazon
{
    public sealed class TextractClient : BaseSubClient<AmazonAIClient, IAmazonTextract, AmazonAIClientOptions>, IDoucmentExtractionClient
    {
        public TextractClient(AmazonAIClient parent) : base(parent) => PlatformClient = Parent.AWSOptions.CreateServiceClient<IAmazonTextract>();

        /// <inheritdoc/>
        public async Task<string> ExtractFromDocument(BinaryData documentData, DocumentExtractionOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Throw.IfNull(documentData);
            Throw.IfNullOrEmpty(documentData.MediaType);
            Throw.IfLessThan(documentData.Length, 1);
            var mergedOptions = options.Merge(ClientOptions.DocumentExtraction);

            using var memoryStream = new MemoryStream(documentData.ToArray());

            var request = new DetectDocumentTextRequest()
            {
                Document = new Document
                {
                    Bytes = memoryStream
                }
            };
            var detectDocTextResponse = await PlatformClient.DetectDocumentTextAsync(request, cancellationToken);
            var stringBuilder = new StringBuilder();
            foreach (var wordBlock in detectDocTextResponse.Blocks.Where(b => b.BlockType == BlockType.WORD))
            {
                stringBuilder.Append(wordBlock.Text + " ");
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
