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
        private const bool AutoComplete = false;

        public static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var threadCount = int.Parse(config["ListeningThreads"]);

            for (var i = 1; i <= threadCount; i++)
            {
                var closure = i;

                var client = new QueueClient(config["ConnectionString"], config["QueuePath"]);

                Task.Run(() =>
                {
                    var id = closure;

                    client.RegisterMessageHandler((message, _) =>
                    {
                        Console.WriteLine($"\n[{id}] Received message: {Encoding.UTF8.GetString(message.Body)}");

                        // ReSharper disable ConditionIsAlwaysTrueOrFalse
                        return AutoComplete ? Task.CompletedTask : client.CompleteAsync(message.SystemProperties.LockToken);
                        // ReSharper restore ConditionIsAlwaysTrueOrFalse
                    },
                    new MessageHandlerOptions(_ => Task.CompletedTask)
                    {
                        AutoComplete = AutoComplete
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
