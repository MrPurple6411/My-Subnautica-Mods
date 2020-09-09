using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;

namespace SpecialtyManifold.Configuration
{
    [Menu("Specialty Manifold")]
    public class Config: ConfigFile
    {
        [Toggle("Effects of multiple tanks?")]
        public bool multipleTanks;
    }
}