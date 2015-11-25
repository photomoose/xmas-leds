namespace Rumr.DurryLights.Domain.Models
{
    public class Colour
    {
        public string Name { get; set; }

        public string HexValue { get; private set; }

        public Colour(string name, string hexValue)
        {
            Name = name;
            HexValue = hexValue;
        }
    }
}