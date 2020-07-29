#if SUBNAUTICA
#elif BELOWZERO
using Newtonsoft.Json;
#endif

namespace CustomHullPlates
{
    public class HullPlateInfo
    {
        public string InternalName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}