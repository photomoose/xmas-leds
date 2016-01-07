namespace Rumr.DurryLights.Domain.Commands
{
    public class FadingInOutLightDisplay : LightDisplay
    {
        protected override string Command
        {
            get { return "f,1000,5"; }
        }
    }
}