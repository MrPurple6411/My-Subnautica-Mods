namespace ExtraOptions.Configuration
{
    using SMLHelper.V2.Json;
    using SMLHelper.V2.Options.Attributes;

    [Menu("ExtraOptions")]
    public class Config: ConfigFile
    {
        [Slider("Murkiness", 0, 400, Step = 10, Tooltip = "How clear the water is to allow for distance viewing."), OnChange(nameof(ApplyOptions))]
        public float Murkiness = 200.0f;

        [Slider("LODGroupBias", 1, 10, Step = 1f, Tooltip = "In simple terms how far you can see objects and terrain.\nIncreaseing `can` decrease performance."), OnChange(nameof(ApplyOptions))]
        public int LODGroupBias = 1;

        [Choice("Texture Quality", new[] { "0", "1", "2", "3", "4" }, Tooltip = "Will effect performance."), OnChange(nameof(ApplyOptions))]
        public int TextureQuality = 3;

        [Slider("Shadow LOD", 200, 1000, Step = 100, Tooltip = "This affects all of the light particles or shaders on plants/terrain.\nDecreasing `can` increase performance."), OnChange(nameof(ApplyOptions))]
        public int ShaderLOD = 400;

        [Choice("Shadow Cascades", new[] { "1", "2", "4" }, Tooltip = "How perfect the edges of shadows are shaped.\nDecreasing `can` increase performance."), OnChange(nameof(ApplyOptions))]
        public int ShadowCascades = 2;

        [Toggle("AmbientParticles", Tooltip = "The small, clear, spherical particles hovering in the water which can be seen during the daytime.\nDisabling `can` increase performance.")]
        public bool AmbientParticles = true;

        [Toggle("LightShaft", Tooltip = "enables and disables sun rays coming into the water."), OnChange(nameof(ApplyOptions))]
        public bool LightShafts = true;

        [Toggle("Variable Physics Step"), OnChange(nameof(ApplyOptions))]
        public bool VariablePhysicsStep = true;

        [Toggle("Fog \"Fix\""), OnChange(nameof(ApplyOptions))]
        public bool FogFix = true;

        [Toggle("Clear Water Surface", Tooltip = "Makes the surface of the water clearer so you can see more when looking down from above the surface."), OnChange(nameof(ApplyOptions))]
        public bool ClearSurface = true;

        private void ApplyOptions()
        {
            Main.ApplyOptions();
        }
    }
}