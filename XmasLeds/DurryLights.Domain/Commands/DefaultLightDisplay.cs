namespace Rumr.DurryLights.Domain.Commands
{
    public class DefaultLightDisplay : LightDisplay
    {
        protected override string Command
        {
            get { return "D,1000,0,4000"; }
        }
    }
}