namespace Rumr.DurryLights.Domain.Commands
{
    public class Cycle2LightDisplay : LightDisplay
    {
        protected override string Command
        {
            get { return "c,100"; }
        }         
    }
}