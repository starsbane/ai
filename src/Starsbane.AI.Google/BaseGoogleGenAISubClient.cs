using Google.Apis.Auth.OAuth2;
using Google.GenAI;

namespace Starsbane.AI.Google
{
    public abstract class BaseGoogleGenAISubClient : BaseGoogleSubClient<Client>
    {
        protected string _providerName => (ClientOptions.IsVertex) ? "google.vertexai" : "google.geminiai";

        protected BaseGoogleGenAISubClient(GoogleAIClient parent) : base(parent)
        {
            if (ClientOptions.IsVertex)
            {
                GoogleCredential? credential = null;
                if (!string.IsNullOrEmpty(ClientOptions.GoogleApplicationCredentials))
                {
                    credential = GoogleCredential.FromFile(ClientOptions.GoogleApplicationCredentials);
                    if (credential.IsCreateScopedRequired)
                    {
                        credential = credential.CreateScoped("https://www.googleapis.com/auth/cloud-platform");
                    }
                }
                else
                {
                    credential = GoogleCredential.GetApplicationDefault();
                }

                PlatformClient = new Client(true, credential: credential, project: ClientOptions.VertexAIProjectName, location: ClientOptions.VertexAILocation);
            }
            else
            {
                PlatformClient = new Client(false, ClientOptions.GeminiApiKey ?? ClientOptions.GoogleApiKey);
            }
        }

        public void Dispose() => PlatformClient?.Dispose();
    }
}
