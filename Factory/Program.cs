using System;
using RabbitMQ.Client;
using System.Text;
using CoffeeFactory.Models;
using Newtonsoft.Json;
using System.Linq;

namespace CoffeeFactory
{
    class Program
    {
        private static int interval = 5000;
        private static string hostName = "localhost";
        private static string queueName = "CoffeeQueue";
        private static readonly System.Timers.Timer timer = new();

        static void Main(string[] args)
        {
            
            CheckArgs(args);
            timer.Interval = interval;
            timer.Elapsed += Produce;
            timer.Start();

            while (true) 
            {
                var key = Console.ReadKey();
                if (key.KeyChar == 'q')
                {
                    timer.Dispose();
                    break;
                }
            }

            Console.WriteLine("press on q to quit or u to reproduce.");
            Console.ReadKey();
        }

        /// <summary>
        /// Produces one coffee and sends it to the message queue.
        /// </summary>
        private static void Produce(object sender, EventArgs ew)
        {
            var factory = new ConnectionFactory() { HostName = hostName };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var coffee = new Coffee();
            var json = JsonConvert.SerializeObject(coffee);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine($"Coffee Id:{coffee.Id}, Produced At:{coffee.ProducedAt} has been sent.");
        }

        /// <summary>
        /// Checks the optional arguments that might be given on executing the program.
        /// </summary>
        /// <param name="args">The arguments array.</param>
        private static void CheckArgs(string[] args) 
        {
            var argList = args.ToList();
            var index = -1;
            if ((index = argList.IndexOf("--every")) != -1 && argList.Count > ++index)
            {
                var success = int.TryParse(args[index], out int ms);
                interval = success ? ms : 5000;
            }
            index = -1;
            if ((index = argList.IndexOf("--hostname")) != -1 && argList.Count > ++index)
            {
                hostName = args[index];
            }
            if ((index = argList.IndexOf("--queueName")) != -1 && argList.Count > ++index)
            {
                queueName = args[index];
            }
        }
    }
}
