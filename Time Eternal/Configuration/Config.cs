using System;
using System.Collections.Generic;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;

namespace Time_Eternal.Configuration
{
    [Menu("Time Eternal", LoadOn = MenuAttribute.LoadEvents.MenuRegistered | MenuAttribute.LoadEvents.MenuOpened)]
    public class Config: ConfigFile
    {
        public List<String> validOptions = new List<string>() { "0 = Normal", "1 = Noon", "2 = MidNight" };

        [Choice(Label = "Freeze Lighting", Options = new string[] { "Normal", "Noon", "MidNight" })]
        public int freezeTimeChoice;
    }
}