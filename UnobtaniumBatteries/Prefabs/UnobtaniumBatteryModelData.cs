using CustomBatteries.API;
using SMLHelper.V2.Utility;
using System.IO;
using UnityEngine;

namespace UnobtaniumBatteries.Prefabs
{
    internal class UnobtaniumBatteryModelData: CBModelData
    {
        public override Texture2D CustomTexture { get; } = ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "battery_skin.png"));
        public override Texture2D CustomNormalMap { get; } = ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "battery_normal.png"));
        public override Texture2D CustomSpecMap { get; } = ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "battery_spec.png"));
        public override Texture2D CustomIllumMap { get; } = ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "battery_illum.png"));

        public override float CustomIllumStrength => 1f;

        public override bool UseIonModelsAsBase => true;
    }
}
