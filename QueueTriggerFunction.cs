using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MyFunctionApp
{
    public class QueueTriggerFunction
    {
        private readonly ILogger _logger;

        public QueueTriggerFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<QueueTriggerFunction>();
        }

        [Function("QueueTriggerFunction")]
        public void Run([QueueTrigger("your-queue-name", Connection = "AzureWebJobsStorage")] string myQueueItem)
        {
            var message = JsonSerializer.Deserialize<YourMessageType>(myQueueItem);
            _logger.LogInformation($"C# Queue trigger function processed: {message.Text}");
        }
    }
}
