using System.Threading.Tasks;
using Rumr.DurryLights.Domain.Models;

namespace Rumr.DurryLights.Domain
{
    public interface IMetricWriter
    {
        Task SendAsync(Metric metric);
    }
}