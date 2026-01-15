namespace Starsbane.AI.Google
{
    public abstract class BaseGoogleSubClient<TSubClient>(GoogleAIClient parent) : BaseSubClient<GoogleAIClient, TSubClient, GoogleAIClientOptions>(parent) where TSubClient : class
    {
    }
}
