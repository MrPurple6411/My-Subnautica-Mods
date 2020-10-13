using System;
using System.Collections.Generic;
using System.IO;
using CustomBatteries.API;
using InfinityBattery.MonoBehaviours;
using SMLHelper.V2.Utility;
using UnityEngine;
#if SN1
using Sprite = Atlas.Sprite;
#endif

namespace InfinityBattery.Prefabs
{
    internal class InfinityBatteryItem : CbItem
    {
        public override int EnergyCapacity => 1000000;

        public override string ID => "InfinityBattery";

        public override string Name => "Infinity Battery";

        public override string FlavorText => "Battery that constantly keeps 1 Million Power";

        public override IList<TechType> CraftingMaterials => new List<TechType>();

        public override TechType UnlocksWith => TechType.None;

        public override Sprite CustomIcon => Main.Icon;

        public override Texture2D CustomTexture => Main.Texture;

        public override Texture2D CustomIllumMap => Main.Illum;

        public override float CustomIllumStrength => 1f;

        public override bool UseIonModels => true;

        public override Texture2D CustomNormalMap => Main.Normal;

        public override Texture2D CustomSpecMap => Main.Spec;

        public override bool ExcludeFromChargers => true;

        public override void EnhanceGameObject(GameObject gameObject)
        {
            gameObject.EnsureComponent<InfinityBehaviour>();
        }
    }
}
