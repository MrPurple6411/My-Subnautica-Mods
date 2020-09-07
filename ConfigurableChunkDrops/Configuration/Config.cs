using SMLHelper.V2.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurableChunkDrops.Configuration
{
    public class Config: ConfigFile
    {
        public Config(string fileName = "config", string subfolder = null) : base(fileName, subfolder)
        {
        }

        public Dictionary<TechType, Dictionary<TechType, float>> Breakables = new Dictionary<TechType, Dictionary<TechType, float>>();
    }
}
