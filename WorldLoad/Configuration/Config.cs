using SMLHelper.V2.Json;
using SMLHelper.V2.Options;

namespace WorldLoad.Configuration
{
    [Menu("World Load Settings")]
    public class Config: ConfigFile
    {
        [Slider("Load Distance", 2, 50, DefaultValue = 8, Step = 1)]
        public int IncreasedWorldLoad = 8;
    }
}