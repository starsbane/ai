using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Starsbane.AI;

namespace AIClientSamples
{
    internal class ChatSample(
        ILogger<ChatSample> logger,
        IEnumerable<IAIClient> clients)
        : ISample
    {
        private string OutputPath => Path.Combine(Directory.GetCurrentDirectory(), "Generated");

        public async Task RunSample()
        {
            Directory.CreateDirectory(OutputPath);


            foreach (var client in clients)
            {
                var tasks = new List<Task>();
                tasks.Add(Task.Run(async () =>
                {
                    logger.LogInformation($"Getting chat response...");
                    var chatResponse = await client.GetChatClient().GetResponseAsync("What is the birth date of Albert Einstein?");
                    logger.LogInformation(
                        $"Output: {chatResponse.Text}");
                    logger.LogInformation($"Chat response retrieved. ");
                }));

                tasks.Add(Task.Run(async () =>
                {
                    logger.LogInformation($"Getting streaming response...");
                    await foreach (var chatResponseUpdate in client.GetChatClient().GetStreamingResponseAsync("What is the birth date of Albert Einstein?"))
                    {
                        logger.LogInformation($"Chat Response Updated, text: {chatResponseUpdate.Text}");
                    }

                    logger.LogInformation($"streaming response retrieved. ");
                }));
                await Task.WhenAll(tasks);
            }
        }
    }
}
