using System.Collections.Generic;

namespace Rumr.DurryLights.Domain.Commands
{
    public abstract class LightDisplay
    {
        private readonly List<string> _colours = new List<string>();

        public IList<string> Colours
        {
            get { return _colours.AsReadOnly(); }
        }

        public void AddColour(string hexValue)
        {
            _colours.Add(hexValue);
        }
    }
}