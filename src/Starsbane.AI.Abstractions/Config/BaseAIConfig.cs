using System.Collections.ObjectModel;

namespace Starsbane.AI
{
    public abstract class BaseAIConfig : IPropertyEnvVariableMapping
    {
        public virtual ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping { get; }
    }

    public abstract class BaseAIConfig<TChatConfig, TEmbeddingConfig, TImageConfig, TVideoConfig, TSpeechConfig, TTextExtractionConfig, TDocumentExtractionConfig> : BaseAIConfig
        where TChatConfig : ChatOptionConfig
        where TEmbeddingConfig : EmbeddingGenerationConfig
        where TImageConfig: ImageGenerationConfig
        where TVideoConfig: VideoGenerationConfig
        where TSpeechConfig: SpeechGenerationConfig
        where TTextExtractionConfig: TextExtractionConfig
        where TDocumentExtractionConfig: DocumentExtractionConfig
    {
        private TChatConfig? _chat;
        private TEmbeddingConfig? _embedding;
        private TImageConfig? _image;
        private TVideoConfig? _video;
        private TSpeechConfig? _speech;
        private TTextExtractionConfig? _textExtraction;
        private TDocumentExtractionConfig? _documentExtraction;

        protected BaseAIConfig()
        {
            if (PropertyEnvVariableMapping != null)
            {
                foreach (var mapping in PropertyEnvVariableMapping)
                {
                    var propInfo = GetType().GetProperty(mapping.Key);
                    if (propInfo == null) continue;

                    if (propInfo.GetValue(this) != null) continue;
                    foreach (var environmentVariable in mapping.Value)
                    {
                        var envValue = Environment.GetEnvironmentVariable(environmentVariable);
                        if (envValue == null) continue;
                        propInfo.SetValue(this, envValue);
                        break;
                    }
                }
            }

            AIClientExtensions.SetPropertiesInObject(ref _chat);
            AIClientExtensions.SetPropertiesInObject(ref _chat);
            AIClientExtensions.SetPropertiesInObject(ref _embedding);
            AIClientExtensions.SetPropertiesInObject(ref _image);
            AIClientExtensions.SetPropertiesInObject(ref _video);
            AIClientExtensions.SetPropertiesInObject(ref _speech);
            AIClientExtensions.SetPropertiesInObject(ref _textExtraction);
            AIClientExtensions.SetPropertiesInObject(ref _documentExtraction);
        }

        public TChatConfig? Chat
        {
            get => _chat;
            set => _chat = value;
        }

        public TEmbeddingConfig? Embedding
        {
            get => _embedding;
            set => _embedding = value;
        }

        public TImageConfig? Image
        {
            get => _image;
            set => _image = value;
        }

        public TVideoConfig? Video
        {
            get => _video;
            set => _video = value;
        }

        public TSpeechConfig? Speech
        {
            get => _speech;
            set => _speech = value;
        }

        public TTextExtractionConfig? TextExtraction
        {
            get => _textExtraction;
            set => _textExtraction = value;
        }

        public TDocumentExtractionConfig? DocumentExtraction
        {
            get => _documentExtraction;
            set => _documentExtraction = value;
        }
    }
}
