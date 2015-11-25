using System.Collections.Generic;
using System.Threading.Tasks;
using Rumr.DurryLights.Domain.Models;

namespace Rumr.DurryLights.Domain.Repositories
{
    public interface IColourRepository
    {
        Task<IEnumerable<Colour>> GetColoursAsync();
    }
}