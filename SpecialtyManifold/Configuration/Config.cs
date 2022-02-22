namespace SpecialtyManifold.Configuration
{
    using SMCLib.Json;
    using SMCLib.Options.Attributes;

    [Menu("Specialty Manifold")]
    public class Config: ConfigFile
    {
        [Toggle("Effects of multiple tanks?")]
        public bool multipleTanks;
    }
}