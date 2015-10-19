using System.Collections.Generic;
using System.Linq;

namespace Rumr.DurryLights.Domain.LightDisplays
{
    public abstract class LightDisplay
    {
        private readonly List<string> _colours = new List<string>();

        public IList<string> Colours
        {
            get { return _colours.AsReadOnly(); }
        }

        public void AddColours(IEnumerable<Colour> colours)
        {
            _colours.AddRange(colours.Select(c => c.HexValue));
        }
    }
}