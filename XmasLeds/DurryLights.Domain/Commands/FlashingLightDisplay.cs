﻿namespace Rumr.DurryLights.Domain.Commands
{
    public class FlashingLightDisplay : LightDisplay
    {
        protected override string Command
        {
            get { return "F,1000,1000"; }
        }
    }
}