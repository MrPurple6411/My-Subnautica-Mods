namespace BuilderModule.Module
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using SMLHelper.Assets;
    using SMLHelper.Assets.Interfaces;
    using SMLHelper.Crafting;
    using SMLHelper.Utility;
    using UnityEngine;
    using static CraftData;

#if SN1
    using Sprite = Atlas.Sprite;
#endif


    internal class BuilderModulePrefab: IEquipable, ICraftable, IModPrefab
    {
        public PrefabInfo PrefabInfo { get; private set; }
        private static Sprite Sprite { get; } = ImageUtils.LoadSpriteFromFile($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Assets/BuilderModule.png");

        public BuilderModulePrefab()
        {
            PrefabInfo = PrefabInfo.Create("BuilderModule").CreateTechType()
                .WithLanguageLines("Builder Module", "Allows you to build bases while in your vehicle."); 
            if(Sprite != null)
                PrefabInfo.WithIcon(Sprite);
            PrefabInfo.RegisterPrefab(this);
        }

        public TechGroup GroupForPDA => TechGroup.VehicleUpgrades;

        public TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;

#if SN1
        public CraftTree.Type FabricatorType => CraftTree.Type.SeamothUpgrades;
#elif BZ
        public CraftTree.Type FabricatorType => CraftTree.Type.SeaTruckFabricator;
#endif
        public EquipmentType EquipmentType => EquipmentType.VehicleModule;
        public QuickSlotType QuickSlotType => QuickSlotType.Toggleable;

        public string[] StepsToFabricatorTab { get; } = new[] { "ExosuitModules" };


        public RecipeData RecipeData { get; } = new()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>(new Ingredient[]
                {
                    new(TechType.Builder, 1),
                    new(TechType.AdvancedWiringKit, 1)
                })
        };

        public float CraftingTime => 2f;

        System.Func<IOut<GameObject>, IEnumerator> IModPrefab.GetGameObjectAsync => GetGameObjectAsync;

        public IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.VehicleStorageModule, false);

            yield return task;
            var prefab = EditorModifications.Instantiate(task.GetResult(), default, default, false);
            prefab.GetComponentsInChildren<UniqueIdentifier>().ForEach((x)=> { if(x is PrefabIdentifier) x.classId = PrefabInfo.ClassID; else Object.DestroyImmediate(x.gameObject); });
            if(prefab.TryGetComponent(out TechTag tag)) tag.type = PrefabInfo.TechType;
            Object.DestroyImmediate(prefab.GetComponent<SeamothStorageContainer>());

            gameObject.Set(prefab);
        }
    }
}