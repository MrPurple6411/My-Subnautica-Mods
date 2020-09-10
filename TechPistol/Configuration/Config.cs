using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;

namespace TechPistol.Configuration
{
    [Menu("TechPistol")]
    public class Config : ConfigFile
    {
        [Slider("Cannon Damage", 1, 1000, DefaultValue = 150, Step = 10)]
        public float CannonDamage = 150f;

        [Slider("Cannon Explosion Range", 1, 100, DefaultValue = 15, Step = 1)]
        public float CannonExplosionDamageRange = 15f;

        [Slider("Laser Damage", 1, 100, DefaultValue = 2, Step = 1)]
        public float LaserDamage = 2f;

        [Slider("Laser Range", 1, 100, DefaultValue = 40, Step = 1)]
        public float LaserRange = 40f;

        [Slider("Health Detection Range", 1, 100, DefaultValue = 40, Step = 10)]
        public float HealthDetectionRange = 40f;

        [Slider("Resize Range", 1, 100, DefaultValue = 150, Step = 10)]
        public float ScaleRange = 40f;

        [Slider("Increase Size Speed", 0.01f, 0.3f, DefaultValue = 0.02f, Step = 0.01f)]
        public float ScaleUpspeed = 0.02f;

        [Slider("Decrease Size Speed", 0.01f, 0.3f, DefaultValue = 0.02f, Step = 0.01f)]
        public float ScaleDownspeed = 0.02f;
    }
}