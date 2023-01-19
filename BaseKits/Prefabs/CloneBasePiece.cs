namespace BaseKits.Prefabs
{
    using SMLHelper.Assets;
    using SMLHelper.Assets.Interfaces;
    using SMLHelper.Crafting;
    using SMLHelper.Handlers;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using static CraftData;

    internal class CloneBasePiece : IBuildable, IModPrefab
    {
        private static readonly List<TechType> UnlockRequired = new()
        {
            TechType.BaseBulkhead, TechType.BaseRoom, TechType.BaseMapRoom, TechType.BaseMoonpool,
            TechType.BaseBioReactor, TechType.BaseNuclearReactor, TechType.BaseObservatory, TechType.BaseUpgradeConsole,
            TechType.BaseFiltrationMachine, TechType.BaseWaterPark, TechType.BaseLargeRoom, TechType.BaseLargeGlassDome,
            TechType.BaseGlassDome, TechType.BaseControlRoom, TechType.BasePartition, TechType.BasePartitionDoor
        };

        internal PrefabInfo PrefabInfo { get; private set; }

        private GameObject processedPrefab;
        private readonly TechType TypeToClone;
        private readonly TechType KitTechType;
        private readonly TechGroup group;
        private readonly TechCategory category;
        public TechGroup GroupForPDA => group;

        public TechCategory CategoryForPDA => category;
        public RecipeData RecipeData { get; }

        Func<IOut<GameObject>, IEnumerator> IModPrefab.GetGameObjectAsync => GetGameObjectAsync;

        internal CloneBasePiece(TechType typeToClone, TechType kitTechType)
        {
            TypeToClone = typeToClone;
            KitTechType = kitTechType;
            RecipeData = new() { craftAmount = 1, Ingredients = new List<Ingredient>() { new(KitTechType, 1) } };

            PrefabInfo = PrefabInfo.Create($"CBP_{typeToClone}").CreateTechType().WithIcon(SpriteManager.Get(TypeToClone))
                .WithLanguageLines($"{Language.main.Get(typeToClone)}", "Built from a Kit!");
            CraftDataHandler.SetBackgroundType(PrefabInfo.TechType, CraftData.BackgroundType.PlantAir);

            if(UnlockRequired.Contains(typeToClone))
            {
                PrefabInfo.WithUnlock(typeToClone);
            }
            CraftData.GetBuilderIndex(typeToClone, out group, out category, out _);
            PrefabInfo.RegisterPrefab(this);
        }

        public IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            GameObject go = null;
            if (processedPrefab != null)
            {
                go = GameObject.Instantiate(processedPrefab);
                go.SetActive(true);
                gameObject.Set(go);
                yield break;
            }

            var task = CraftData.GetPrefabForTechTypeAsync(TypeToClone);
            yield return task;

            processedPrefab = task.GetResult();
            go = GameObject.Instantiate(processedPrefab);
            go.SetActive(true);
            gameObject.Set(go);
        }
    }
}