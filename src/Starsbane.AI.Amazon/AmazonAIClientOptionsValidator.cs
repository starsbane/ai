using Microsoft.Extensions.Options;

namespace Starsbane.AI.Amazon
{
    public class AmazonAIClientOptionsValidator : IValidateOptions<AmazonAIClientOptions>
    {
        public ValidateOptionsResult Validate(string? name, AmazonAIClientOptions options)
        {
            if (options.Video?.VideoOutputS3Uri != null && options.Video?.VideoOutputS3Uri.Scheme.ToLower() != "s3")
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Video.VideoOutputS3Uri)} must start with \"s3://\".");
            }

            if (options.Embedding?.TokenLimit is <= 0 or > 8192)
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Embedding.TokenLimit)} must be in range of 1 and 8192.");
            }

            if (options.Embedding?.Dimensions is not null and not 256 and not 512 and not 1024)
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Embedding.Dimensions)} must be either 256, 512 or 1024.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
