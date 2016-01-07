using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rumr.DurryLights.Domain.Commands
{
    public abstract class LightDisplay
    {
        private const int MaxColours = 20;
        private readonly List<string> _colours = new List<string>();

        protected abstract string Command { get; }

        public IList<string> Colours
        {
            get { return _colours.AsReadOnly(); }
        }

        public void AddColour(string hexValue)
        {
            _colours.Add(hexValue);
        }

        public string Serialize()
        {
            var sb = new StringBuilder(Command);

            foreach (var colour in Colours.Take(MaxColours))
            {
                sb.Append("," + colour);
            }

            return sb.ToString();
        }
    }
}