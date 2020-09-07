using System;
using System.Collections.Generic;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options;

namespace Time_Eternal.Configuration
{
    [Menu("Time Eternal", IgnoreUnattributedMembers = true)]
    public class Config: ConfigFile
    {
        public List<String> validOptions = new List<string>() { "0 = Normal", "1 = Noon", "2 = MidNight" };

        [Choice(Label = "Freeze Lighting", Options = new string[] { "Normal", "Noon", "MidNight" })]
        public int freezeTimeChoice;
    }
}