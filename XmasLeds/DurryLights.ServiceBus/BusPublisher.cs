using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Rumr.DurryLights.Domain.Commands;
using Rumr.DurryLights.Domain.Messaging;

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

        public async Task PublishAsync(LightDisplay lightDisplay)
        {
            await PublishAsync(lightDisplay, DateTime.MinValue);
        }

        public async Task PublishAsync(LightDisplay lightDisplay, DateTime scheduledEnqueueTimeUtc)
        {
            var topicClient = _factory.CreateTopicClient("Commands");

            var message = lightDisplay.Serialize();
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(message));
            var brokeredMessage = new BrokeredMessage(ms);
            brokeredMessage.Properties.Add("MessageType", lightDisplay.GetType().Name);

            if (scheduledEnqueueTimeUtc != DateTime.MinValue)
            {
                brokeredMessage.ScheduledEnqueueTimeUtc = scheduledEnqueueTimeUtc;
            }
            
            await topicClient.SendAsync(brokeredMessage);
        }
    }
}
