namespace Rumr.DurryLights.Domain.Commands
{
    public class StrobeLightDisplay : LightDisplay
    {
        protected override string Command
        {
            get { return "s,60,4000"; }
        }
    }
}