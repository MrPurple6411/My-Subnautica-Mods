using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomCreatureSize.Configuration
{
    [Menu("Random Creature Size")]
    class Config: ConfigFile
    {
        [Slider("Min Size", 0.1f, 10f, Format = "{0:P0}")]
        public float minsize = 0.1f;
        [Slider("Max Size", 0.1f, 10f, Format = "{0:P0}")]
        public float maxsize = 2f;
    }
}
