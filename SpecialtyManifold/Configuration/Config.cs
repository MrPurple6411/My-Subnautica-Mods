namespace SpecialtyManifold.Configuration
{
    using SMLHelper.Json;
    using SMLHelper.Options.Attributes;

    [Menu("Specialty Manifold")]
    public class SMLConfig: ConfigFile
    {
        [Toggle("Effects of multiple tanks?")]
        public bool multipleTanks;
    }
}