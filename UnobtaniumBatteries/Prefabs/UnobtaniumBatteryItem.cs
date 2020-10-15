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
    internal class UnobtaniumBatteryItem : CbItem
    {
        public override int EnergyCapacity => 1000000;

        public override string ID => "UnobtaniumBattery";

        public override string Name => "Unobtanium Battery";

        public override string FlavorText => "Battery that constantly keeps 1 Million Power";

        public override IList<TechType> CraftingMaterials { get; } = new List<TechType>() { TechType.ReaperLeviathan, TechType.GhostLeviathan, TechType.Warper };

        public override TechType UnlocksWith => TechType.Warper;

        public override Sprite CustomIcon { get; } = ImageUtils.LoadSpriteFromFile(Path.Combine(Main.AssetsFolder, "battery_icon.png"));

        public override CBModelData CBModelData => new UnobtaniumBatteryModelData();

        public override bool ExcludeFromChargers => true;

        public override void EnhanceGameObject(GameObject gameObject)
        {
            gameObject.EnsureComponent<InfinityBehaviour>();
        }
    }
}
