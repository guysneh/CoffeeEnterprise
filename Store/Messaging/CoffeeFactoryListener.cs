using CoffeeStore.Data;
using CoffeeStore.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CoffeeStore.Messaging
{
    /// <summary>
    /// Service responsible for listening to the message queue and update the DB when new coffees sent from the coffee factory.
    /// </summary>
    public class CoffeeFactoryListener : BackgroundService, IDisposable
    {
        /// <summary>
        /// The name of the message queue
        /// </summary>
        private string _queueName;

        /// <summary>
        /// The channel used for listening to the broker of the message queue. 
        /// </summary>
        private IModel _channel;
        
        /// <summary>
        /// The scope factory injected service. Used for getting the CoffeeStore DbContext.
        /// </summary>
        private readonly IServiceScopeFactory scopeFactory;

        /// <summary>
        /// The class constructor.
        /// </summary>
        /// <param name="scopeFactory">The scope factory injected service.</param>
        /// <param name="configuration">The configration injected service</param>
        /// <exception cref="ArgumentNullException">Thrown if the _channel member is null after initialsing the class.</exception>
        public CoffeeFactoryListener(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            this.scopeFactory = scopeFactory;
            _queueName = configuration.GetValue<string>(Constants.ConfigurationKeys.QueueName);
            InitializeRabbitMqListener(configuration);
            if (_channel == null) 
            {
                throw new ArgumentNullException();
            }
        }

        /// <inheritdoc/>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
           {
               var content = Encoding.UTF8.GetString(ea.Body.ToArray());
               var coffee = JsonConvert.DeserializeObject<Coffee>(content);

               if (coffee != null)
               {
                   await SaveCoffeeToDb(coffee);
                }
           };

            _channel.BasicConsume(_queueName, true, consumer);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Saves the coffee that sent from the message queue(from the coffee factory) to the Coffee-Store Database.
        /// </summary>
        /// <param name="coffee">The new coffee that is to be saved.</param>
        private async Task SaveCoffeeToDb(Coffee coffee)
        {
            using var scope = scopeFactory.CreateScope();
            var coffeeStoreContext = scope.ServiceProvider.GetRequiredService<CoffeeStoreContext>();
            coffeeStoreContext.Coffees.Add(coffee);
            await coffeeStoreContext.SaveChangesAsync();
        }

        /// <summary>
        /// Initializes the RabbitMQ (message queue) listener.
        /// </summary>
        /// <param name="configuration">The configuration injected service object.</param>
        private void InitializeRabbitMqListener(IConfiguration configuration)
        {
            var factory = new ConnectionFactory() { HostName = configuration.GetValue<string>(Constants.ConfigurationKeys.QueueHostName) };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }
    }
}
