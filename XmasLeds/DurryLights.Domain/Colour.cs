namespace Rumr.DurryLights.Domain
{
    public class Colour
    {
        public string Name { get; set; }

        public byte R { get; set; }

        public byte G { get; set; }
        
        public byte B { get; set; }

        public Colour(string name, byte r, byte g, byte b)
        {
            Name = name;
            R = r;
            G = g;
            B = b;
        }
    }
}