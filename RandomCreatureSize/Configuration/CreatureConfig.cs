namespace RandomCreatureSize.Configuration
{
    using SMLHelper.Json;
    using System.Collections.Generic;

    internal class CreatureConfig: ConfigFile
    {
        public CreatureConfig() : base("CreatureConfig")
        {
        }

        public Dictionary<string, float> CreatureSizes = new();

    }
}
