namespace SpecialtyManifold.Configuration
{
    using SMLHelper.V2.Json;
    using SMLHelper.V2.Options.Attributes;

    [Menu("Specialty Manifold")]
    public class SMLConfig: ConfigFile
    {
        [Toggle("Effects of multiple tanks?")]
        public bool multipleTanks;
    }
}