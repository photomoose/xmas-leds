using System.Threading.Tasks;
using Rumr.DurryLights.Domain.Models;

namespace Rumr.DurryLights.Domain.Services
{
    public interface ILightsService
    {
        Task HandleRequestAsync(LightsRequest request);
    }
}