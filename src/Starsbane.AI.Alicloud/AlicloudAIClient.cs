using Cnblogs.DashScope.Core;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace Starsbane.AI.Alicloud
{
    public class AlicloudAIClient : BaseGenAIClient<AlicloudAIClientOptions>, IAIClient<AlicloudAIClientOptions>
    {
        internal readonly DashScopeClient Client;
        internal readonly string ApiKey;

        // Constructor
        public AlicloudAIClient(AlicloudAIClientOptions clientOptions)
        {
            ClientOptions = clientOptions;
            AIClientExtensions.SetPropertiesInObject(ref _clientOptions);

            Throw.IfNullOrEmpty(ClientOptions.DashscopeApiKey);

            Client = new DashScopeClient(ClientOptions.DashscopeApiKey);
        }

        public AlicloudAIClient(IOptions<AlicloudAIClientOptions> options) : this(options.Value)
        {
        }

        public Platform Platform => Platform.Aliyun;
        public IChatClient GetChatClient()
        {
            return new DashScopeChatClient(this);
        }

        public IAIEmbeddingGenerator<string, Embedding<float>> GetEmbeddingGenerator()
        {
            return new DashScopeEmbeddingGenerator(this);
        }

        public IVideoGenerator GetVideoGenerator()
        {
            return new DashScopeVideoGenerator(this);
        }

        public ISpeechGenerator GetSpeechGenerator()
        {
            return new DashScopeSpeechGenerator(this);
        }

        public IImageGenerator GetImageGenerator()
        {
            return new DashScopeImageGenerator(this);
        }

        public IOCRClient GetOCRClient()
        {
            return new DashScopeMultimodalGenerationClient(this);
        }

        public IDoucmentExtractionClient GetDocumentExtractionClient()
        {
            return new DashScopeMultimodalGenerationClient(this);
        }
    }
}
