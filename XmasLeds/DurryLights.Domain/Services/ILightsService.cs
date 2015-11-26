using System.Threading.Tasks;
using Rumr.DurryLights.Domain.Commands;
using Rumr.DurryLights.Domain.Models;

namespace Rumr.DurryLights.Domain.Services
{
    public interface ILightsService
    {
        Task<LightsResponse> HandleRequestAsync(LightsRequest request);
    }
}