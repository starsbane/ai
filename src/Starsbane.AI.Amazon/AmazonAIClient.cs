using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Starsbane.AI.Amazon
{
    public class AmazonAIClient : BaseGenAIClient<AmazonAIClientOptions>, IAIClient<AmazonAIClientOptions>
    {
        internal readonly AmazonAIClientOptions ClientOptions;
        internal readonly AWSOptions AWSOptions;

        public AmazonAIClient(AmazonAIClientOptions clientOptions, IConfiguration configuration)
        {
            ClientOptions = clientOptions;
            AIClientExtensions.SetPropertiesInObject(ref _clientOptions);
            AWSOptions = configuration.GetAWSOptions($"{AmazonAIClientOptions.SectionName}:AWS");
        }

        public AmazonAIClient(IOptions<AmazonAIClientOptions> options, IConfiguration configuration) : this(options.Value, configuration)
        {
        }

        public Platform Platform { get; } = Platform.AWS;

        public IChatClient GetChatClient()
        {
            return new BedrockChatClient(this);
        }

        public IAIEmbeddingGenerator<string, Embedding<float>> GetEmbeddingGenerator()
        {
            return new BedrockEmbeddingGenerator(this);
        }

        public IVideoGenerator GetVideoGenerator()
        {
            return new BedrockVideoGenerator(this);
        }

        public ISpeechGenerator GetSpeechGenerator()
        {
            return new PollySpeechGenerator(this);
        }

        public IImageGenerator GetImageGenerator()
        {
            return new BedrockImageGenerator(this);
        }

        public IOCRClient GetOCRClient()
        {
            return new RekognitionClient(this);
        }

        public IDoucmentExtractionClient GetDocumentExtractionClient()
        {
            return new TextractClient(this);
        }
    }
}
