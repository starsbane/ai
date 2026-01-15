using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;


namespace Starsbane.AI.Google
{
    public class GoogleAIClient : BaseGenAIClient<GoogleAIClientOptions>, IAIClient<GoogleAIClientOptions>
    {
        private readonly ISpeechFormatConverter _speechFormatConverter;

        public GoogleAIClient(GoogleAIClientOptions clientOptions, ISpeechFormatConverter speechFormatConverter)
        {
            ClientOptions = clientOptions;
            AIClientExtensions.SetPropertiesInObject(ref _clientOptions);
            _speechFormatConverter = speechFormatConverter;
        }

        public GoogleAIClient(GoogleAIClientOptions clientOptions) : this(clientOptions, new FFmpegSpeechFormatConverter())
        {
        }


        public GoogleAIClient(IOptions<GoogleAIClientOptions> clientOptions, ISpeechFormatConverter speechFormatConverter) : this(clientOptions.Value, speechFormatConverter)
        {
        }

        public Platform Platform { get; } = Platform.GoogleCloud;
        public IChatClient GetChatClient()
        {
            return new GoogleGenAIChatClient(this);
        }

        public IAIEmbeddingGenerator<string, Embedding<float>> GetEmbeddingGenerator()
        {
            return new GoogleGenAIEmbeddingGenerator(this);
        }

        public IVideoGenerator GetVideoGenerator()
        {
            return new GoogleGenAIVideoGenerator(this);
        }

        public ISpeechGenerator GetSpeechGenerator()
        {
            return new GoogleGenAISpeechGeneratorr(this, _speechFormatConverter);
        }

        public IImageGenerator GetImageGenerator()
        {
            return new GoogleGenAIImageGenerator(this);
        }

        public IOCRClient GetOCRClient()
        {
            return new GoogleImageAnnotatorClient(this);
        }

        public IDoucmentExtractionClient GetDocumentExtractionClient()
        {
            return new GoogleDocumentAIClient(this);
        }
    }
}
