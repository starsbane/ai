using Microsoft.Extensions.AI;

namespace Starsbane.AI
{
    public static class ConfigExtensions
    {
        public static ChatOptions? Merge(this ChatOptions? options, ChatOptionConfig? config)
        {
            if (options == null) return config;
            if (config == null) return options;

            options?.ModelId ??= config.ModelId;
            options?.Instructions ??= config.Instructions;
            options?.Temperature ??= config.Temperature;
            options?.MaxOutputTokens ??= config.MaxOutputTokens;
            options?.TopP ??= config.TopP;
            options?.TopK ??= config.TopK;
            options?.FrequencyPenalty ??= config.FrequencyPenalty;
            options?.PresencePenalty ??= config.PresencePenalty;
            options?.Seed ??= config.Seed;
            options?.ResponseFormat ??= config.ResponseFormat;
            options?.ModelId ??= config.ModelId;
            options?.StopSequences ??= config.StopSequences;
            return options;
        }

        public static ImageGenerationConfig? Merge(this ImageGenerationOptions? obj, ImageGenerationConfig? operand)
        {
            if (obj == null) return operand;
            if (operand == null) return obj;

            obj?.Count ??= operand?.Count;
            obj?.ImageSize ??= operand?.ImageSize;
            obj?.MediaType ??= operand?.MediaType;
            obj?.ModelId ??= operand?.ModelId;
            obj?.ResponseFormat ??= operand?.ResponseFormat;
            obj?.StreamingCount ??= operand?.StreamingCount;

            return obj;
        }

        public static EmbeddingGenerationOptions? Merge(this EmbeddingGenerationOptions? obj, EmbeddingGenerationConfig? operand)
        {
            if (obj == null) obj = new EmbeddingGenerationConfig();
            if (operand == null) operand = new EmbeddingGenerationConfig();

            obj?.Dimensions ??= operand?.Dimensions;
            obj?.ModelId ??= operand?.ModelId;

            return obj;
        }

        public static VideoGenerationOptions? Merge(this VideoGenerationOptions? obj, VideoGenerationConfig? operand)
        {
            if (obj == null) obj = new VideoGenerationConfig();
            if (operand == null) operand = new VideoGenerationConfig();

            obj?.FPS = operand?.FPS;
            obj?.Seconds = operand?.Seconds;
            obj?.VideoDimensions = operand?.VideoDimensions;
            obj?.ModelId = operand?.ModelId;
            obj?.ResponseFormat = operand?.ResponseFormat;
            obj?.StreamingCount = operand?.StreamingCount;

            return obj;
        }

        public static SpeechGenerationOptions? Merge(this SpeechGenerationOptions? obj, SpeechGenerationConfig? operand)
        {
            if (obj == null) obj = new SpeechGenerationConfig();
            if (operand == null) operand = new SpeechGenerationConfig();

            obj?.ModelId ??= operand?.ModelId;
            obj?.VoiceId = operand?.VoiceId;
            obj?.ResponseFormat = operand?.ResponseFormat;

            return obj;
        }

        public static TextExtractionOptions? Merge(this TextExtractionOptions? obj, TextExtractionConfig? operand)
        {
            if (obj == null) obj = new TextExtractionConfig();
            if (operand == null) operand = new TextExtractionConfig();

            obj.Language = operand?.Language;

            return obj;
        }

        public static DocumentExtractionOptions? Merge(this DocumentExtractionOptions? obj, DocumentExtractionConfig? operand)
        {
            if (obj == null) obj = new DocumentExtractionConfig();
            if (operand == null) operand = new DocumentExtractionConfig();

            obj.Language = operand?.Language;

            return obj;
        }
    }
}
