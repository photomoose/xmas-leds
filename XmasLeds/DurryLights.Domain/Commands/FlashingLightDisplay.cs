namespace Rumr.DurryLights.Domain.Commands
{
    public class FlashingLightDisplay : LightDisplay
    {
        public int Interval
        {
            get { return 500; }
        }
    }
}