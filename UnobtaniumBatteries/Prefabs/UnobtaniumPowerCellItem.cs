using System.Collections.Generic;
using System.IO;
using CustomBatteries.API;
using UnobtaniumBatteries.MonoBehaviours;
using SMLHelper.V2.Utility;
using UnityEngine;

#if SN1
using Sprite = Atlas.Sprite;
#endif

namespace UnobtaniumBatteries.Prefabs
{
    internal class UnobtaniumPowerCellItem : CbItem
    {
        public override int EnergyCapacity => 1000000;

        public override string ID => "UnobtaniumCell";

        public override string Name => "Unobtanium Cell";

        public override string FlavorText => "Power Cell that constantly keeps 1 Million Power";

        public override IList<TechType> CraftingMaterials { get; } = new List<TechType>() { Main.UnobtaniumBatteryPack.ItemPrefab.TechType };

        public override TechType UnlocksWith => TechType.Warper;

        public override Sprite CustomIcon => ImageUtils.LoadSpriteFromFile(Path.Combine(Main.AssetsFolder, "cell_icon.png"));

        public override CBModelData CBModelData => new UnobtaniumPowerCellModelData();

        public override bool ExcludeFromChargers => true;

        public override void EnhanceGameObject(GameObject gameObject)
        {
            gameObject.EnsureComponent<InfinityBehaviour>();
        }
    }
}
