namespace Rumr.DurryLights.Domain
{
    public class LightDisplay
    {
        private readonly Colour _colour;

        public LightDisplay(Colour colour)
        {
            _colour = colour;
        }

        public Colour Colour
        {
            get { return _colour; }
        }
    }
}