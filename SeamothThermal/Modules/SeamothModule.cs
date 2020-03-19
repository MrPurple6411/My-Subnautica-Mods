using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using UnityEngine;

namespace SeamothThermal
{
    public abstract class SeamothModule : ModPrefab
    {
        public static TechType SeamothThermalModule { get; protected set; }

        public readonly string ID;
        public readonly string DisplayName;
        public readonly string Tooltip;
        public readonly TechType RequiredForUnlock;
        public readonly CraftTree.Type Fabricator;
        public readonly string[] StepsToTab;
        public readonly Atlas.Sprite Sprite;
        public readonly TechType AddAfter;

        protected SeamothModule(string id, string displayName, string tooltip, CraftTree.Type fabricator, string[] stepsToTab, TechType requiredToUnlock = TechType.None, TechType addAfter = TechType.None, Atlas.Sprite sprite = null) : base(id, $"WorldEntities/Tools/{id}", TechType.None)
        {
            ID = id;
            DisplayName = displayName;
            Tooltip = tooltip;
            Fabricator = fabricator;
            RequiredForUnlock = requiredToUnlock;
            StepsToTab = stepsToTab;
            Sprite = sprite;
            AddAfter = addAfter;

            Patch();
        }

        public void Patch()
        {
            TechType = TechTypeHandler.AddTechType(ID, DisplayName, Tooltip, RequiredForUnlock == TechType.None);

            if (RequiredForUnlock != TechType.None)
            {
                KnownTechHandler.SetAnalysisTechEntry(RequiredForUnlock, new TechType[] { TechType });
            }

            if (Sprite == null)
            {
                SpriteHandler.RegisterSprite(TechType, $"./QMods/SeamothThermal/Assets/{ID}.png");
            }
            else
            {
                SpriteHandler.RegisterSprite(TechType, Sprite);
            }

            switch (Fabricator)
            {
                case CraftTree.Type.Workbench:
                    CraftDataHandler.AddToGroup(TechGroup.Workbench, TechCategory.Workbench, TechType, AddAfter);
                    break;

                case CraftTree.Type.SeamothUpgrades:
                    CraftDataHandler.AddToGroup(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades, TechType, AddAfter);
                    break;
            }

            CraftDataHandler.SetEquipmentType(TechType, EquipmentType.SeamothModule);
            CraftDataHandler.SetTechData(TechType, GetTechData());

            CraftTreeHandler.AddCraftingNode(Fabricator, TechType, StepsToTab);
        }

        public override GameObject GetGameObject()
        {
            // Get the ElectricalDefense module prefab and instantiate it
            var path = "WorldEntities/Tools/SeamothElectricalDefense";
            var prefab = Resources.Load<GameObject>(path);
            var obj = GameObject.Instantiate(prefab);

            // Get the TechTags and PrefabIdentifiers
            var techTag = obj.GetComponent<TechTag>();
            var prefabIdentifier = obj.GetComponent<PrefabIdentifier>();

            // Change them so they fit to our requirements.
            techTag.type = TechType;
            prefabIdentifier.ClassId = ClassID;

            return obj;
        }

        public abstract TechData GetTechData();
    }
}
