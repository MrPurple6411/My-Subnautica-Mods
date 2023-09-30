namespace SpecialtyManifold.Configuration;

using Nautilus.Json;
using Nautilus.Options.Attributes;

[Menu("Specialty Manifold")]
public class SMLConfig: ConfigFile
{
    [Toggle("Effects of multiple tanks?")]
    public bool multipleTanks;
}