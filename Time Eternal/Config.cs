using System;
using System.Collections.Generic;
using SMLHelper.V2.Json;

namespace Time_Eternal
{
    public class Config: ConfigFile
    {
        public List<String> validOptions = new List<string>() { "0 = Normal", "1 = Noon", "2 = MidNight" };
        public int freezeTimeChoice;
    }
}