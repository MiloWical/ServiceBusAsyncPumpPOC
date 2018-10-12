using System;

namespace ServiceBusReader
{
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.Configuration;

    public class Reader
    {
        public static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var threadCount = int.Parse(config["ListeningThreads"]);

            for (var i = 1; i <= threadCount; i++)
            {
                var closure = i;

                Task.Run(() =>
                {
                    var id = closure;

                    var client = new QueueClient(config["ConnectionString"], config["QueuePath"]);

                    client.RegisterMessageHandler((message, _) =>
                    {
                        Console.WriteLine($"\n[{id}] Received message: {Encoding.UTF8.GetString(message.Body)}");

                        return Task.CompletedTask;
                    },
                    new MessageHandlerOptions(_ => Task.CompletedTask)
                    {
                        AutoComplete = true
                    }
                    );

                    Thread.Sleep(Timeout.Infinite);
                });

                Console.WriteLine($"Thread {closure}/{threadCount} configured and running...");
            }

            Console.WriteLine("Configuration complete. Listening for messages...");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
