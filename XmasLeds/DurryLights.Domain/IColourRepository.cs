using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Threading.Tasks;

namespace Rumr.DurryLights.Domain
{
    public interface IColourRepository
    {
        Task<Colour> FindColourAsync(string colourName);

        Task<IEnumerable<Colour>> FindColoursAsync(IEnumerable<string> colourNames);
    }
}