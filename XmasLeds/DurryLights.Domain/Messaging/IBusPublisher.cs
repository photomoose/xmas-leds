using System;
using System.Threading.Tasks;
using Rumr.DurryLights.Domain.Commands;

namespace Rumr.DurryLights.Domain.Messaging
{
    public interface IBusPublisher
    {
        Task InitializeAsync();

        Task PublishAsync(LightDisplay lightDisplay);

        Task PublishAsync(LightDisplay lightDisplay, DateTime scheduledEnqueueTimeUtc);
    }
}
