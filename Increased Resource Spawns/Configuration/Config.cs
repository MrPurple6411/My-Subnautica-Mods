using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;

namespace Increased_Resource_Spawns.Configuration
{
    [Menu("Increased Resource Spawn Settings")]
    public class Config: ConfigFile
    {
        [Slider("Resource Multiplier", 1, 100, Step = 1)]
        public int ResourceMultiplier = 1;
    }
}