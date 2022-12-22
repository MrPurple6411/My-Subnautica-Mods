namespace ConfigurableChunkDrops.Configuration
{
    using SMLHelper.V2.Json;
    using System.Collections.Generic;

    public class SMLConfig: ConfigFile
    {
        public SMLConfig(string fileName = "config", string subfolder = null) : base(fileName, subfolder)
        {
        }

        public Dictionary<string, Dictionary<string, float>> Breakables = new();
    }
}
