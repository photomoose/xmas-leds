using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Rumr.DurryLights.Domain;

namespace Rumr.DurryLights.ServiceBus
{
    public class BusPublisher : IBusPublisher
    {
        private readonly string _connectionString;
        private readonly MessagingFactory _factory;

        public BusPublisher(string connectionString)
        {
            _connectionString = connectionString;
            _factory = MessagingFactory.CreateFromConnectionString(connectionString);
        }

        public async Task InitializeAsync()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);

            var topicExists = await namespaceManager.TopicExistsAsync("Commands");

            if (!topicExists)
            {
                await namespaceManager.CreateTopicAsync("Commands");
            }            
        }

        public async Task PublishAsync<T>(T message)
        {
            var topicClient = _factory.CreateTopicClient("Commands");

            var json = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
            var brokeredMessage = new BrokeredMessage(json);
            brokeredMessage.Properties.Add("MessageType", message.GetType().Name);

            await topicClient.SendAsync(brokeredMessage);
        }
    }
}
