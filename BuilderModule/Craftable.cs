using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace BuilderModule
{
    ///<summary>
    ///Based on Craftable Class from PrimeSonic/Upgraded Vehicles mod
    ///Original code found on GitHub: https://github.com/PrimeSonic/PrimeSonicSubnauticaMods/blob/master/UpgradedVehicles/Craftables/Craftable.cs
    ///</summary>

    internal abstract class Craftable: ModPrefab
    {
        public readonly string NameID;
        public readonly string NameUsingForFiles;
        public readonly string FriendlyName;
        public readonly string Description;
        public GameObject _GameObject { get; private set; }

        protected readonly TechType PrefabTemplate;
        protected readonly CraftTree.Type FabricatorType;
        protected readonly string FabricatorTab;
        protected readonly TechType RequiredForUnlock;
        protected readonly TechGroup GroupForPDA;
        protected readonly TechCategory CategoryForPDA;
        protected readonly EquipmentType TypeForEquipment;
        protected readonly QuickSlotType TypeForQuickslot;
        protected readonly Vector2int ItemSize;
        protected readonly string GameResourceFileName;

        protected Craftable(
            string nameID,
            string nameUsingForFiles,
            string friendlyName,
            string description,
            TechType template,
            CraftTree.Type fabricatorType,
            string fabricatorTab,
            TechType requiredAnalysis,
            TechGroup groupForPDA,
            TechCategory categoryForPDA,
            EquipmentType equipmentType,
            QuickSlotType quickSlotType,
            Vector2int itemSize,
            string gamerResourceFileName
            )
            : base(nameID, $"{nameID}Prefab")
        {
            NameID = nameID;
            NameUsingForFiles = nameUsingForFiles;
            FriendlyName = friendlyName;
            Description = description;
            PrefabTemplate = template;
            FabricatorType = fabricatorType;
            FabricatorTab = fabricatorTab;
            RequiredForUnlock = requiredAnalysis;
            GroupForPDA = groupForPDA;
            CategoryForPDA = categoryForPDA;
            TypeForEquipment = equipmentType;
            TypeForQuickslot = quickSlotType;
            ItemSize = itemSize;
            GameResourceFileName = gamerResourceFileName;
        }

        public virtual void Patch()
        {
#if SUBNAUTICA
            Atlas.Sprite sprite;
#elif BELOWZERO
            Sprite sprite;
#endif
            if(NameUsingForFiles != null)
            {
                sprite = ImageUtils.LoadSpriteFromFile($"./QMods/{NameUsingForFiles}/Assets/{NameUsingForFiles}.png");
            }
            else
            {
                sprite = GetResourceIcon(PrefabTemplate);
            }

            TechType = TechTypeHandler.AddTechType(NameID, FriendlyName, Description, sprite, false);
            SpriteHandler.RegisterSprite(TechType, sprite);
            CraftTreeHandler.AddCraftingNode(FabricatorType, TechType, FabricatorTab);
            CraftDataHandler.SetTechData(TechType, GetRecipe());
            CraftDataHandler.AddToGroup(GroupForPDA, CategoryForPDA, TechType);
            CraftDataHandler.SetEquipmentType(TechType, TypeForEquipment);
            CraftDataHandler.SetQuickSlotType(TechType, TypeForQuickslot);
            CraftDataHandler.SetItemSize(TechType, ItemSize);

            KnownTechHandler.SetAnalysisTechEntry(RequiredForUnlock, new TechType[1] { TechType }, $"{FriendlyName} blueprint discovered!");

            PrefabHandler.RegisterPrefab(this);
        }

#if SUBNAUTICA

        protected abstract TechData GetRecipe();

#elif BELOWZERO

        protected abstract RecipeData GetRecipe();

#endif

        public override GameObject GetGameObject()
        {
            if(GameResourceFileName == null)
            {
                _GameObject = Object.Instantiate(CraftData.GetPrefabForTechType(PrefabTemplate));
            }
            else
            {
                _GameObject = Object.Instantiate(Resources.Load<GameObject>(GameResourceFileName));
            }

            _GameObject.name = NameID;

            return _GameObject;
        }

#if SUBNAUTICA

        public Atlas.Sprite GetResourceIcon(TechType techType)
#elif BELOWZERO

        public Sprite GetResourceIcon(TechType techType)
#endif
        {
            return SpriteManager.Get(techType);
        }
    }
}