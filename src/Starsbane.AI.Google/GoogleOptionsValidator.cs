using Microsoft.Extensions.Options;

namespace Starsbane.AI.Google
{
    public class GoogleAIClientOptionsValidator : IValidateOptions<GoogleAIClientOptions>
    {
        public ValidateOptionsResult Validate(string? name, GoogleAIClientOptions options)
        {
            if (string.IsNullOrEmpty(options.VertexAILocation))
            {
                return ValidateOptionsResult.Fail(
                    $"{nameof(options.VertexAILocation)} must be provided when {nameof(options.IsVertex)} is true.");
            }
            else if (string.IsNullOrEmpty(options.VertexAIProjectName))
            {
                return ValidateOptionsResult.Fail(
                    $"{nameof(options.VertexAIProjectName)} must be provided when {nameof(options.IsVertex)} is true.");
            }


            if (options.Embedding?.TokenLimit is <= 0 or > 2048)
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Embedding.TokenLimit)} must be in range of 1 and 2048.");
            }

            if (options.Embedding?.Dimensions is <= 0 or > 3072)
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Embedding.Dimensions)} must be between 1 and 3072.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
