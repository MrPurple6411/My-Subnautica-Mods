using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Increased_Resource_Spawns.Configuration
{
    [Menu("Increased Resource Spawn Settings")]
    public class Config: ConfigFile
    {
        [Slider("Resource Multiplier", 1, 100, Step = 1)]
        public int ResourceMultiplier = 1;

        public List<string> Blacklist = new List<string>() { TechType.CrashHome.AsString(), TechType.SpikePlant.AsString() };

        public List<string> WhiteList = new List<string>() { };
    }
}