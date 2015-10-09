using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Rumr.DurryLights.Domain;

namespace Rumr.DurryLights.Sql
{
    public class ColourRepository : IColourRepository
    {
        public async Task<Domain.Colour> FindColourAsync(string colourName)
        {
            using (var db = new LightsContext())
            {
                var colour = await db.Colours.SingleOrDefaultAsync(c => c.Name.Equals(colourName.ToLower()));

                return colour != null ? new Domain.Colour(colour.Name, colour.R, colour.G, colour.B) : null;
            }
        }

        public async Task<IEnumerable<Domain.Colour>> FindColoursAsync(IEnumerable<string> colourNames)
        {
            using (var db = new LightsContext())
            {
                var colours = await db.Colours
                    .Where(c => colourNames.Contains(c.Name))
                    .ToListAsync();

                return colours.Select(c => new Domain.Colour(c.Name, c.R, c.G, c.B));
            }
        }
    }
}