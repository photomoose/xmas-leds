using Rumr.DurryLights.Domain.Commands;

namespace Rumr.DurryLights.Domain.Utilities
{
    public class LightDisplayFactory
    {
        public LightDisplay Create(string command)
        {
            switch (command.ToLower())
            {
                case "fade":
                    return new FadingInOutLightDisplay();

                case "fadeout":
                    return new FadeOutLightDisplay();

                case "strobe":
                    return new StrobeLightDisplay();
                    
                case "flash":
                    return new FlashingLightDisplay();

                case "cycle":
                    return new Cycle1LightDisplay();

                case "cycle2":
                    return new Cycle2LightDisplay();

                case "night":
                    return new NightLightDisplay();

                case "twinkle":
                    return new TwinkleLightDisplay();

                default:
                    return new DefaultLightDisplay();
            }
        }
    }
}