using Cnblogs.DashScope.Core;

namespace Starsbane.AI.Alicloud
{
    public sealed class DashScopeMultimodalGenerationClient(AlicloudAIClient parent) : BaseDashScopeSubClient(parent), IOCRClient, IDoucmentExtractionClient
    {
        /// <inheritdoc/>
        public async Task<string> ExtractFromImage(BinaryData imageData, TextExtractionOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Throw.IfNull(imageData);
            Throw.IfNullOrEmpty(imageData.MediaType);
            Throw.IfLessThan(imageData.Length, 1);
            var mergedOptions = options.Merge(ClientOptions.TextExtraction);

            List<MultimodalMessage> messages =
            [
                MultimodalMessage.User([
                    MultimodalMessageContent.ImageContent(imageData.ToArray(), imageData.MediaType)
                ])
            ];
            var response = await PlatformClient.GetMultimodalGenerationAsync(
                new ModelRequest<MultimodalInput, IMultimodalParameters>()
                {
                    Model = "qwen-vl-ocr-latest",
                    Input = new MultimodalInput { Messages = messages },
                    Parameters = new MultimodalParameters()
                    {
                        OcrOptions = new()
                        {
                            Task = "text_recognition"
                        }
                    }
                }, cancellationToken);

            return response.Output.Choices[0].Message.Content[0].Text ?? "";
        }

        /// <inheritdoc/>
        public async Task<string> ExtractFromDocument(BinaryData documentData, DocumentExtractionOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Throw.IfNull(documentData);
            Throw.IfNullOrEmpty(documentData.MediaType);
            Throw.IfLessThan(documentData.Length, 1);
            var mergedOptions = options.Merge(ClientOptions.DocumentExtraction);

            List<MultimodalMessage> messages =
            [
                MultimodalMessage.User([
                    MultimodalMessageContent.ImageContent(documentData.ToArray(), documentData.MediaType)
                ])
            ];
            var response = await PlatformClient.GetMultimodalGenerationAsync(
                new ModelRequest<MultimodalInput, IMultimodalParameters>()
                {
                    Model = "qwen-vl-ocr-latest",
                    Input = new MultimodalInput { Messages = messages },
                    Parameters = new MultimodalParameters()
                    {
                        OcrOptions = new()
                        {
                            Task = "document_parsing"
                        }
                    }
                }, cancellationToken);

            return response.Output.Choices[0].Message.Content[0].Text ?? "";
        }
    }
}
