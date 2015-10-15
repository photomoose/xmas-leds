using System.Collections.Generic;
using System.Linq;

namespace Rumr.DurryLights.Domain
{
    public class LightDisplay
    {
        private readonly List<string> _colours;

        public LightDisplay()
        {
            _colours = new List<string>();
        }

        public LightDisplay(IEnumerable<Colour> colours)
        {
            _colours = new List<string>(colours.Select(c => c.HexValue));
        }

        public IList<string> Colours
        {
            get { return _colours.AsReadOnly(); }
        }
    }
}