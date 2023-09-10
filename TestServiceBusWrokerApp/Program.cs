using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace TestServiceBusWrokerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            builder.Services.AddHostedService<Worker>();
            
            


            var app = builder.Build();

            app.MapGet("/api/test", () => "Hello World!");


            int numThreads = Environment.ProcessorCount; // Use all available CPU cores

            Thread[] threads = new Thread[numThreads];

            for (int i = 0; i < numThreads; i++)
            {
                threads[i] = new Thread(PerformCPUIntensiveTask);
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }





            app.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });

        static void PerformCPUIntensiveTask()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Perform a CPU-intensive task (e.g., calculating prime numbers)
            long maxPrime = 1000000;
            for (long num = 2; num <= maxPrime; num++)
            {
                bool isPrime = true;
                for (long divisor = 2; divisor <= Math.Sqrt(num); divisor++)
                {
                    if (num % divisor == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime)
                {
                    // Uncomment the following line to print prime numbers (may produce a lot of output)
                    // Console.WriteLine(num);
                }
            }

            stopwatch.Stop();
        }
    }
}