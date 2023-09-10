using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestServiceBusWrokerApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _serviceBusConnection;
        private readonly string _topicName = "testsubsc";

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceBusConnection = "Endpoint=sb://testazureservicebusjsp1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=6UnNpzPcybwqRqm1ChzZduCtPB8ORUbbe+ASbNmyCuY=";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await using var client = new ServiceBusClient(_serviceBusConnection);
            var processor = client.CreateProcessor("testtopic", "testsubsc", new ServiceBusProcessorOptions());
            processor.ProcessMessageAsync += async args =>
        {
            var message = args.Message;
            string messageBody = Encoding.UTF8.GetString(message.Body);

            _logger.LogInformation($"Received message: {messageBody}");

            // Process the message as needed

            await args.CompleteMessageAsync(message);
        };

            processor.ProcessErrorAsync += args =>
            {
                _logger.LogError(args.Exception, "An error occurred while processing the message.");
                return Task.CompletedTask;
            };


            await processor.StartProcessingAsync(stoppingToken);
            Console.WriteLine("Wait for a minute and then press any key to end the processing");
            Console.ReadKey();

            // The processor will handle message retrieval and processing
            // create a timer delay task to wait indefinite
            // Allow the service to run until cancellation
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(100); // Add a small delay or other work if needed
            }

            // Stop and dispose of the processor when the service is shutting down
            await processor.StopProcessingAsync(stoppingToken);
            await processor.DisposeAsync();

        }
    }
}

