using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Queues;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyFunctionApp
{
    public class HttpTriggerFunction
    {
        private readonly ILogger _logger;
        private readonly QueueClient _queueClient;

        public HttpTriggerFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpTriggerFunction>();

            // Получаем строку подключения из переменных окружения
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string queueName = "your-queue-name"; // Название очереди
            _queueClient = new QueueClient(connectionString, queueName);
        }

        [Function("HttpTriggerFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var message = JsonSerializer.Deserialize<YourMessageType>(requestBody);

            await _queueClient.CreateIfNotExistsAsync();
            if (await _queueClient.ExistsAsync())
            {
                await _queueClient.SendMessageAsync(JsonSerializer.Serialize(message));
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Message added to the queue");

            return response;
        }
    }

    public class YourMessageType
    {
        public string Text { get; set; }
    }
}
