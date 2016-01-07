namespace Rumr.DurryLights.Domain.Commands
{
    public class FadeOutLightDisplay : LightDisplay
    {
        private const char CommandChar = 'o';
        private const int NumFadeIncrements = 1000;
        private const int FadeIncrementDuration = 0;
        private const int OnDurationMs = 100;
        private const int OffDurationMs = 1000;

        protected override string Command
        {
            get { return string.Format("{0},{1},{2},{3},{4}", CommandChar, NumFadeIncrements, FadeIncrementDuration, OnDurationMs, OffDurationMs); }
        }
    }
}