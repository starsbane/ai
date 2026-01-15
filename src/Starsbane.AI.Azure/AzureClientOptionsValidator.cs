using Microsoft.Extensions.Options;

namespace Starsbane.AI.Azure
{
    public class AzureClientOptionsValidator : IValidateOptions<AzureAIClientOptions>
    {
        public ValidateOptionsResult Validate(string? name, AzureAIClientOptions options)
        {
            if (options.Embedding?.TokenLimit is not null and (<= 0 or > 8191))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Embedding.TokenLimit)} must be in range of 1 and 8191.");
            }

            if (options.Embedding?.Dimensions is <= 0 or > 3072 )
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Embedding.Dimensions)} must be between 1 and 3072.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
