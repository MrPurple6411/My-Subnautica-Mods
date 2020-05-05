using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using UnityEngine;

namespace CustomFragmentExample
{
    public class Exosuite2 : Craftable
    {
        private GameObject Prefab { get; set; }
        private TechType TechToCopy { get; set; }

        private TechType UnlockedBy { get; set; }

        public override TechType RequiredForUnlock => UnlockedBy;

        public override bool AddScannerEntry => true;

        public override int FragmentsToScan => 1;

        public override float TimeToScanFragment => 5;

        public override bool DestroyFragmentOnScan => true;

        public override TechGroup GroupForPDA => TechGroup.Constructor;

        public override TechCategory CategoryForPDA => TechCategory.Constructor;

        public override string DiscoverMessage => "HELLS YES WE SCANNED IT!";

        public override CraftTree.Type FabricatorType => CraftTree.Type.Constructor;

        public override string[] StepsToFabricatorTab => base.StepsToFabricatorTab;

        public override float CraftingTime => 1f;

        public Exosuite2(string classId, string friendlyName, string description, GameObject prefab = null, TechType techToCopy = TechType.None, TechType unlockedBy = TechType.None) : base(classId, friendlyName, description)
        {
            Prefab = prefab ?? (techToCopy != TechType.None ? CraftData.GetPrefabForTechType(techToCopy, false) : CraftData.GetPrefabForTechType(TechType.None, false));
            TechToCopy = techToCopy;
            UnlockedBy = unlockedBy;
        }

        public override GameObject GetGameObject()
        {
            GameObject _GameObject = UnityEngine.Object.Instantiate(Prefab);
            _GameObject.name = ClassID;

            _GameObject.transform.localScale *= 2f;

            return _GameObject;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return CraftDataHandler.GetTechData(TechToCopy) ?? new TechData();
        }

        protected override Atlas.Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechToCopy);
        }
    }
}
