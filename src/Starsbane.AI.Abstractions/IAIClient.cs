using Microsoft.Extensions.AI;




namespace Starsbane.AI
{
    public interface IAIClient
    {
        Platform Platform { get; }

        IChatClient GetChatClient();

        IAIEmbeddingGenerator<string, Embedding<float>> GetEmbeddingGenerator();

        IVideoGenerator GetVideoGenerator();

        ISpeechGenerator GetSpeechGenerator();

        IImageGenerator GetImageGenerator();

        IOCRClient GetOCRClient();

        IDoucmentExtractionClient GetDocumentExtractionClient();
    }

    public interface IAIClient<TClientOptions> : IAIClient
        where TClientOptions : BaseAIConfig
    {
        public TClientOptions ClientOptions { get; set; }
    }
}
