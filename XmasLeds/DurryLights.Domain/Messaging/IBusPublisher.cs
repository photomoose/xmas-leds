using System;
using System.Threading.Tasks;

namespace Rumr.DurryLights.Domain.Messaging
{
    public interface IBusPublisher
    {
        Task InitializeAsync();

        Task PublishAsync<T>(T message);

        Task PublishAsync<T>(T message, DateTime scheduledEnqueueTimeUtc);
    }
}
