using System.Collections.Generic;
using SMLHelper.V2.Json;

namespace BetterACU
{
    internal class Config : ConfigFile
    {
        public int WaterParkSize;
#if BELOWZERO
        public int LargeWaterParkSize;
#endif
        public bool OverFlowIntoOcean;
        public Dictionary<string, float> PowerValues = new Dictionary<string, float>();

    }
}
