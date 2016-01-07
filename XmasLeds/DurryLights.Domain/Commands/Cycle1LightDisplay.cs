namespace Rumr.DurryLights.Domain.Commands
{
    public class Cycle1LightDisplay : LightDisplay
    {
        protected override string Command
        {
            get { return "c,1000"; }
        }
    }
}