using CustomBatteries.API;
using SMLHelper.V2.Utility;
using System.IO;
using UnityEngine;

namespace UnobtaniumBatteries.Prefabs
{
    internal class UnobtaniumBatteryModelData: CBModelData
    {
        public override Texture2D CustomTexture => ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "battery_skin.png"));
        public override Texture2D CustomNormalMap => ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "battery_skin_normal.png"));
        public override Texture2D CustomSpecMap => ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "battery_skin_spec.png"));
        public override Texture2D CustomIllumMap => ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "battery_skin_illum.png"));

        public override float CustomIllumStrength => 1f;

        public override bool UseIonModelsAsBase => true;
    }
}
