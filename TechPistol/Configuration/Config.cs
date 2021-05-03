namespace TechPistol.Configuration
{
    using SMLHelper.V2.Json;
    using SMLHelper.V2.Options.Attributes;

    [Menu("TechPistol")]
    public class Config: ConfigFile
    {
        [Slider("Targeting Range", 1, 200, DefaultValue = 40, Step = 1)]
        public float TargetingRange = 40f;

        [Slider("Cannon Damage", 0, 1000, DefaultValue = 150, Step = 10, Tooltip = "Divide this by Explosion Size and Multiply by Charge Cost.")]
        public float CannonDamage = 150f;

        [Slider("Cannon Explosion Size", 1, 100, DefaultValue = 15, Step = 1)]
        public float CannonExplosionSize = 15f;

        [Slider("Cannon Charge Speed", 1, 10, DefaultValue = 1, Step = 1)]
        public float CannonChargeSpeed = 1f;

        [Slider("Laser Damage", 0, 100, DefaultValue = 2, Step = 1, Tooltip = "Damage = Power cost per second")]
        public float LaserDamage = 1f;

        [Toggle("Lethal Resizing")]
        public bool LethalResizing = true;

        [Slider("Scale Kill Size", 10, 100, DefaultValue = 10, Step = 1)]
        public int ScaleKillSize = 10;

        [Slider("Increase Size Speed", 0.01f, 0.3f, DefaultValue = 0.01f, Step = 0.01f, Format = "{0:F2}")]
        public float ScaleUpspeed = 0.01f;

        [Slider("Decrease Size Speed", 0.01f, 0.3f, DefaultValue = 0.01f, Step = 0.01f, Format = "{0:F2}")]
        public float ScaleDownspeed = 0.01f;
    }
}