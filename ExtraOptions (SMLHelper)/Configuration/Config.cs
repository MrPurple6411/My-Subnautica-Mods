using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;

namespace ExtraOptions.Configuration
{
    [Menu("ExtraOptions")]
    public class Config : ConfigFile
    {
        [Slider("Murkiness", 0, 200, Step = 1)]
        public float Murkiness = 100.0f;

        [Choice("Texture Quality", new string[] { "0", "1", "2", "3", "4" })]
        public int TextureQuality = 3;

        [Toggle("LightShaft")]
        public bool LightShafts = true;

        [Toggle("Variable Physics Step")]
        public bool VariablePhysicsStep = true;

        [Toggle("Fog \"Fix\"")]
        public bool FogFix = false;

        [Toggle("Clear Water Surface")]
        public bool ClearSurface = true;
    }
}