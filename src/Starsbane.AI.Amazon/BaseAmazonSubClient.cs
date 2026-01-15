using Amazon.Runtime;

namespace Starsbane.AI.Amazon
{
    public abstract class BaseAmazonSubClient<TAmazonService> : BaseSubClient<AmazonAIClient, TAmazonService, AmazonAIClientOptions>
        where TAmazonService : class, IAmazonService
    {
        protected BaseAmazonSubClient(AmazonAIClient parent) : base(parent)
        {
            PlatformClient = parent.AWSOptions.CreateServiceClient<TAmazonService>();
        }

        public virtual void Dispose()
        {
            // nothing to dispose
        }
    }
}
