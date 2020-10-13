using CustomBatteries.API;
using UnityEngine;

namespace InfinityPowerCell.Prefabs
{
    internal class InfinityPowerCellModelData : CBModelData
    {
        public override Texture2D CustomTexture => Main.Texture;

        public override Texture2D CustomIllumMap => Main.Illum;

        public override float CustomIllumStrength => 1f;

        public override bool UseIonModelsAsBase => true;

        public override Texture2D CustomNormalMap => Main.Normal;

        public override Texture2D CustomSpecMap => Main.Spec;
    }
}
