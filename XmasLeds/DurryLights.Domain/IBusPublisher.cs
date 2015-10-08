using System.Threading.Tasks;

namespace Rumr.DurryLights.Domain
{
    public interface IBusPublisher
    {
        Task InitializeAsync();

        Task PublishAsync<T>(T message);
    }
}
