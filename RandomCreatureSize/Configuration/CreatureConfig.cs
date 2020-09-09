using SMLHelper.V2.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomCreatureSize.Configuration
{
    internal class CreatureConfig : ConfigFile
    {
        public CreatureConfig() : base("CreatureConfig")
        {
        }

        public Dictionary<string, float> CreatureSizes = new Dictionary<string, float>();

    }
}
