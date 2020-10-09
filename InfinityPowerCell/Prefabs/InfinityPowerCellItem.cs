using System.Collections.Generic;
using CustomBatteries.API;
using InfinityPowerCell.MonoBehaviours;
using UnityEngine;
#if SN1
using Sprite = Atlas.Sprite;
#endif

namespace InfinityPowerCell.Prefabs
{
    internal class InfinityPowerCellItem : CbItem
    {
        public override int EnergyCapacity => 1000000;

        public override string ID => "InfinityCell";

        public override string Name => "Infinity Cell";

        public override string FlavorText => "Power Cell that constantly keeps 1 Million Power";

        public override IList<TechType> CraftingMaterials => new List<TechType>();

        public override TechType UnlocksWith => TechType.None;

        public override Sprite CustomIcon => Main.Icon;

        public override Texture2D CustomSkin => Main.Skin;

        public override bool ExcludeFromChargers => true;

        public override void EnhanceGameObject(GameObject gameObject)
        {
            gameObject.EnsureComponent<InfinityBehaviour>();
        }
    }
}
