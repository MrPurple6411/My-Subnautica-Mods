#if SUBNAUTICA
#elif BELOWZERO
using Newtonsoft.Json;
#endif

namespace CustomPosters
{
    public class PosterInfo
    {
        public string InternalName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Orientation { get; set; }
    }
}