using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;

namespace BetterACU.Configuration
{
    [Menu("Better ACU", LoadOn = MenuAttribute.LoadEvents.MenuOpened, SaveOn = MenuAttribute.SaveEvents.ChangeValue)]
    public class Config : ConfigFile
    {

        [Toggle("Allow Breed Into Ocean")]
        public bool OverFlowIntoOcean = true;

        [Slider("Alien Containment Limit", 10, 100, DefaultValue = 10, Step = 1)]
        public int WaterParkSize = 10;

#if BZ
        [Slider("Large Room Alien Containment Limit", 20, 200, DefaultValue = 20, Step = 1)]
        public int LargeWaterParkSize = 20;
#endif
        public Dictionary<string, float> PowerValues = new Dictionary<string, float>();
    }
}
