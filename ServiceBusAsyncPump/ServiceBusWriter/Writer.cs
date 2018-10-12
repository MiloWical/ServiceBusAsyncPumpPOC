namespace ServiceBusWriter
{
    using System;
    using System.Text;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.Configuration;

    public class Writer
    {
        public static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var client = new QueueClient(config["ConnectionString"], config["QueuePath"]);

            Console.Write("Enter a message to transmit (nothing to quit): ");

            var input = Console.ReadLine();

            while (!string.IsNullOrEmpty(input))
            {
                client.SendAsync(new Message(Encoding.UTF8.GetBytes(input))).GetAwaiter().GetResult();
                Console.WriteLine("Message sent.");

                Console.Write("\nEnter a message to transmit (nothing to quit): ");

                input = Console.ReadLine();
            } 
        }
    }
}
