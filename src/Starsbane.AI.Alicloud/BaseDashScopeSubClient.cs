using Cnblogs.DashScope.Core;
using Cnblogs.DashScope.Core.Internals;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Starsbane.AI.Alicloud
{
    public abstract class BaseDashScopeSubClient : BaseSubClient<AlicloudAIClient, DashScopeClient, AlicloudAIClientOptions>
    {
        protected readonly HttpClient HttpClient;
        protected static readonly string _providerName = "alibabacloud.dashscope";

        protected BaseDashScopeSubClient(AlicloudAIClient parent) : base(parent)
        {
            PlatformClient = parent.Client;
            HttpClient = GetHttpClient(parent.ApiKey);
        }

        protected static HttpRequestMessage BuildRequest<TPayload>(
            HttpMethod method,
            string url,
            TPayload? payload = null,
            bool sse = false,
            bool isTask = false)
            where TPayload : class
        {
            var message = new HttpRequestMessage(method, url)
            {
                Content = payload != null
                    ? JsonContent.Create(payload, options: DashScopeDefaults.SerializationOptions)
                    : null
            };

            if (sse)
            {
                message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
            }

            if (isTask)
            {
                message.Headers.Add("X-DashScope-Async", "enable");
            }

            if (payload is IDashScopeOssUploadConfig ossConfig && ossConfig.EnableOssResolve())
            {
                message.Headers.Add("X-DashScope-OssResourceResolve", "enable");
            }

            return message;
        }

        protected async Task<TResponse?> SendAsync<TResponse>(HttpRequestMessage message, CancellationToken cancellationToken)
            where TResponse : class
        {
            var response = await GetSuccessResponseAsync(
                message,
                HttpCompletionOption.ResponseContentRead,
                cancellationToken);
            return await response.Content.ReadFromJsonAsync<TResponse>(
                DashScopeDefaults.SerializationOptions,
                cancellationToken);
        }

        protected async Task<HttpResponseMessage> GetSuccessResponseAsync(
            HttpRequestMessage message,
            HttpCompletionOption completeOption = HttpCompletionOption.ResponseContentRead,
            CancellationToken cancellationToken = default)
        {
            return await GetSuccessResponseAsync<DashScopeError>(message, f => f, completeOption, cancellationToken);
        }

        protected async Task<HttpResponseMessage> GetSuccessResponseAsync<TError>(
            HttpRequestMessage message,
            Func<TError, DashScopeError> errorMapper,
            HttpCompletionOption completeOption = HttpCompletionOption.ResponseContentRead,
            CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;
            try
            {
                response = await HttpClient.SendAsync(message, completeOption, cancellationToken);
            }
            catch (Exception e)
            {
                throw new DashScopeException(message.RequestUri?.ToString(), 0, null, e.Message);
            }

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            DashScopeError? error = null;
            try
            {
                var r = await response.Content.ReadFromJsonAsync<TError>(
                    DashScopeDefaults.SerializationOptions,
                    cancellationToken);
                error = r == null ? null : errorMapper.Invoke(r);
            }
            catch (Exception)
            {
                // ignore
            }

            await ThrowDashScopeExceptionAsync(error, message, response, cancellationToken);
            // will never reach here
            return response;
        }

        [DoesNotReturn]
        protected static async Task ThrowDashScopeExceptionAsync(
            DashScopeError? error,
            HttpRequestMessage message,
            HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            var errorMessage = error?.Message ?? await response.Content.ReadAsStringAsync(cancellationToken);
            throw new DashScopeException(
                message.RequestUri?.ToString(),
                (int)response.StatusCode,
                error,
                errorMessage);
        }

        public virtual void Dispose()
        {
            // nothing to dispose
        }

        private static HttpClient GetHttpClient(
            string apiKey,
            TimeSpan? timeout = null,
            string? baseAddress = null,
            string? workspaceId = null)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress ?? DashScopeDefaults.HttpApiBaseAddress),
                Timeout = timeout ?? TimeSpan.FromMinutes(2)
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.DefaultRequestHeaders.Add("X-DashScope-WorkSpace", workspaceId);

            return client;
        }
    }
}
