using System;
using Rumr.DurryLights.Domain.LightDisplays;

namespace Rumr.DurryLights.Domain
{
    public class LightDisplayFactory
    {
        public LightDisplay Create(string command)
        {
            switch (command.ToLower())
            {
                case "fade":
                    return new FadingInOutLightDisplay();

                case "strobe":
                    return new StrobeLightDisplay();
                    
                case "flash":
                    return new FlashingLightDisplay();

                case "cycle":
                    return new CyclingLightDisplay();

                default:
                    return new DefaultLightDisplay();
            }
        }
    }
}