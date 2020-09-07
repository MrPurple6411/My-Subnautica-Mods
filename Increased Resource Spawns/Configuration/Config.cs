using SMLHelper.V2.Json;
using SMLHelper.V2.Options;

namespace Increased_Resource_Spawns.Configuration
{
    [Menu("Increased Resource Spawn Settings")]
    public class Config: ConfigFile
    {
        [Slider("Resource Multiplier", 1, 100, Format = "{0:P0}", Step = 1)]
        public int ResourceMultiplier = 1;
    }
}