using CustomBatteries.API;
using SMLHelper.V2.Utility;
using System.IO;
using UnityEngine;

namespace UnobtaniumBatteries.Prefabs
{
    internal class UnobtaniumPowerCellModelData : CBModelData
    {
        public override Texture2D CustomTexture => ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "cell_skin.png"));
        public override Texture2D CustomNormalMap => ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "cell_normal.png"));
        public override Texture2D CustomSpecMap => ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "cell_spec.png"));
        public override Texture2D CustomIllumMap => ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "cell_illum.png"));

        public override float CustomIllumStrength => 1f;

        public override bool UseIonModelsAsBase => true;
    }
}
