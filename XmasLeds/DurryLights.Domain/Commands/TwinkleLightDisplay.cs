namespace Rumr.DurryLights.Domain.Commands
{
    public class TwinkleLightDisplay : LightDisplay
    {
        private const char CommandChar = 'f';
        private const int NumFadeIncrements = 175;
        private const int FadeIncrementDuration = 0;

        protected override string Command
        {
            get { return string.Format("{0},{1},{2}", CommandChar, NumFadeIncrements, FadeIncrementDuration); }
        }
    }
}