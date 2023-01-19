namespace BaseKits.Prefabs
{
    using SMLHelper.Assets;
    using SMLHelper.Handlers;
    using System.Collections;
    using System.Collections.Generic;
    using SMLHelper.Crafting;
    using UnityEngine;
    using SMLHelper.Assets.Interfaces;
    using System;

    public class CloneBaseKit : IModPrefab, ICraftable, IPDAInfo
    {
        private static readonly List<TechType> UnlockRequired = new()
        {
            TechType.BaseBulkhead, TechType.BaseRoom, TechType.BaseMapRoom, TechType.BaseMoonpool,
            TechType.BaseBioReactor, TechType.BaseNuclearReactor, TechType.BaseObservatory, TechType.BaseUpgradeConsole,
            TechType.BaseFiltrationMachine, TechType.BaseWaterPark, TechType.BaseLargeRoom, TechType.BaseLargeGlassDome, 
            TechType.BaseGlassDome, TechType.BaseControlRoom, TechType.BasePartition, TechType.BasePartitionDoor
        };

        internal PrefabInfo PrefabInfo { get; }

        private GameObject processedPrefab;
        private readonly TechType TypeToClone;
        private readonly TechGroup group;
        private readonly TechCategory category;

        internal CloneBaseKit(TechType typeToClone, string FabricatorMenu, CraftTree.Type PurpleKitFabricator)
        {
            FabricatorType = PurpleKitFabricator;
            StepsToFabricatorTab = new[] { FabricatorMenu };
            TypeToClone = typeToClone;
            PrefabInfo = PrefabInfo.Create($"Kit_{typeToClone}").CreateTechType().WithIcon(SpriteManager.Get(TypeToClone))
                .WithLanguageLines($"{Language.main.Get(typeToClone)} Kit", "Super Compressed Base in a Kit");

            if(UnlockRequired.Contains(TypeToClone))
                PrefabInfo.WithUnlock(typeToClone);

            RecipeData = CraftDataHandler.GetRecipeData(TypeToClone) ?? new RecipeData() { craftAmount = 0 };
            if (CraftData.GetBuilderIndex(typeToClone, out var originalGroup, out var originalCategory, out _))
            {
                string originalCategoryString = Language.main.Get(uGUI_BlueprintsTab.techCategoryStrings.Get(originalCategory));
                string tgs = $"{originalGroup}_Kits";

                if(!EnumHandler.TryGetModAddedEnumValue(tgs, out group))
                {
                    group = EnumHandler.AddEntry<TechGroup>(tgs).WithPdaInfo($"{originalGroup} - Kits");
                }

                string tcs = $"{originalCategory}_Kits";
                if(!EnumHandler.TryGetModAddedEnumValue(tcs, out category))
                {
                    category = EnumHandler.AddEntry<TechCategory>(tcs).WithPdaInfo($"{originalCategoryString} - Kits").RegisterToTechGroup(group);
                }
            }
            CraftDataHandler.SetBackgroundType(PrefabInfo.TechType, CraftData.BackgroundType.PlantAir);
            PrefabInfo.RegisterPrefab(this);
        }

        public TechGroup GroupForPDA => group;

        public TechCategory CategoryForPDA => category;

        public float CraftingTime => 10f;

        public RecipeData RecipeData { get; }

        public CraftTree.Type FabricatorType { get; }

        public string[] StepsToFabricatorTab { get; }

        Func<IOut<GameObject>, IEnumerator> IModPrefab.GetGameObjectAsync => GetGameObjectAsync;

        public IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            GameObject go = null;
            if (processedPrefab != null)
            {
                go = EditorModifications.Instantiate(processedPrefab, default, default, true);
                gameObject.Set(go);
                yield break;
            }

            var prefab = Utils.CreateGenericLoot(PrefabInfo.TechType);
            if (prefab != null)
            {
                processedPrefab = prefab;
                processedPrefab.SetActive(false);
                go = EditorModifications.Instantiate(processedPrefab, default, default, true);
            }

            gameObject.Set(go);
        }
    }
}