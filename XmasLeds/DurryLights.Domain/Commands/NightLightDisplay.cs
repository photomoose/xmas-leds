namespace Rumr.DurryLights.Domain.Commands
{
    public class NightLightDisplay : LightDisplay
    {
        protected override string Command
        {
            get { return "D,1200,100,10000"; }
        }         
    }
}