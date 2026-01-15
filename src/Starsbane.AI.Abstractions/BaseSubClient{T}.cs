using Microsoft.Extensions.Options;

namespace Starsbane.AI
{
    public abstract class BaseGenAIClient<TClientOptions>
        where TClientOptions : BaseAIConfig
    {
        protected TClientOptions _clientOptions;

        public TClientOptions ClientOptions
        {
            get { return _clientOptions;}
            set { _clientOptions = value; }
        }
    }

    public abstract class BaseSubClient<TGenAIClient, TPlatformClient, TClientOptions>(TGenAIClient parent)
        where TGenAIClient : BaseGenAIClient<TClientOptions>, IAIClient
        where TPlatformClient : class
        where TClientOptions : BaseAIConfig
    {
        protected readonly TGenAIClient Parent = parent;
        protected TPlatformClient PlatformClient { get; set; }
        public TClientOptions ClientOptions => parent.ClientOptions;
    }
}
