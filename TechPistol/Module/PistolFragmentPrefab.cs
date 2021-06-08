#if !EDITOR
namespace TechPistol.Module
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Utility;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UWE;
#if SN1
    using Sprite = Atlas.Sprite;
#endif

    internal class PistolFragmentPrefab: Spawnable
    {

        private static GameObject processedPrefab;
        private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");
        private static readonly int MetallicGlossMap = Shader.PropertyToID("_MetallicGlossMap");
        private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");

        public PistolFragmentPrefab() : base(
            "TechPistolFragment",
            "Damaged Pistol Fragment",
            "Incomplete or Broken fragment of an advanced pistol of unknown origins."
        )
        {
        }

        public override List<LootDistributionData.BiomeData> BiomesToSpawnIn => GetBiomeDistribution();


        private static List<LootDistributionData.BiomeData> GetBiomeDistribution()
        {
#if SN1
            var biomeDatas = new List<LootDistributionData.BiomeData>()
            {
                new(){ biome = BiomeType.DeepGrandReef_AbandonedBase_Interior, count = 1, probability = 0.2f },
                new(){ biome = BiomeType.DeepGrandReef_AbandonedBase_Exterior, count = 1, probability = 0.2f },
                new(){ biome = BiomeType.FloatingIslands_AbandonedBase_Inside, count = 1, probability = 0.2f },
                new(){ biome = BiomeType.FloatingIslands_AbandonedBase_Outside, count = 1, probability = 0.2f },
                new(){ biome = BiomeType.JellyShroomCaves_AbandonedBase_Inside, count = 1, probability = 0.2f },
                new(){ biome = BiomeType.JellyShroomCaves_AbandonedBase_Outside, count = 1, probability = 0.2f },
                new(){ biome = BiomeType.JellyshroomCaves_CaveFloor, count = 1, probability = 0.2f },
                new(){ biome = BiomeType.JellyshroomCaves_CaveSand, count = 1, probability = 0.2f },
                new(){ biome = BiomeType.LostRiverJunction_Ground, count = 1, probability = 0.2f },
                new(){ biome = BiomeType.LostRiverCorridor_Ground, count = 1, probability = 0.2f },
                new(){ biome = BiomeType.LostRiverCorridor_LakeFloor, count = 1, probability = 0.2f },
                new(){ biome = BiomeType.LostRiverJunction_LakeFloor, count = 1, probability = 0.2f },

            };
#elif BZ
            var biomeDatas = new List<LootDistributionData.BiomeData>()
            {
                new LootDistributionData.BiomeData(){ biome = BiomeType.TwistyBridges_Ground, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.TwistyBridges_Deep_Ground, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.TwistyBridges_Cave_Ground, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.TwistyBridges_Deep_ThermalVentArea_Ground, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.GlacialBasin_BikeCrashSite, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.GlacialBasin_Generic, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.GlacialConnection_Ground, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.Glacier_Generic, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.LilyPads_Crevice_Ground, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.LilyPads_Crevice_Grass, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.LilyPads_Deep_Grass, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.LilyPads_Deep_Ground, count = 1, probability = 0.2f },
            };
#endif
            return biomeDatas;
        }

        public override WorldEntityInfo EntityInfo => new() { cellLevel = LargeWorldEntity.CellLevel.Near, classId = ClassID, localScale = Vector3.one, prefabZUp = false, slotType = EntitySlot.Type.Small, techType = TechType };

        public override GameObject GetGameObject()
        {
            if (processedPrefab is not null) return processedPrefab;
            
            var prefab = Main.assetBundle.LoadAsset<GameObject>("TechPistol.prefab");
            var gameObject = Object.Instantiate(prefab, default, default, false);
            
            var componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
            Shader marmo = Shader.Find("MarmosetUBER");
            foreach(var meshRenderer in componentsInChildren)
            {
                if (!meshRenderer.name.StartsWith("Gun") && !meshRenderer.name.StartsWith("Target")) continue;
                var material = meshRenderer.material;

                material.shader = marmo;
                material.EnableKeyword("MARMO_EMISSION");
                material.EnableKeyword("MARMO_SPECMAP");
            }
            
            foreach (var textMesh in gameObject.GetComponentsInChildren<TextMesh>())
            {
                Object.Destroy(textMesh);
            }

            Object.Destroy(gameObject.GetComponentInChildren<ChildObjectIdentifier>().gameObject);
            
            Object.DestroyImmediate(gameObject.GetComponent<PistolBehaviour>());
            Object.DestroyImmediate(gameObject.GetComponent<EnergyMixin>());
            Object.DestroyImmediate(gameObject.GetComponent<VFXFabricating>());
            
            var prefabIdentifier = gameObject.GetComponent<PrefabIdentifier>();
            prefabIdentifier.ClassId = ClassID;
            gameObject.GetComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
            gameObject.GetComponent<TechTag>().type = TechType;

            var pickupable = gameObject.GetComponent<Pickupable>();
            pickupable.isPickupable = false;

            var resourceTracker = gameObject.EnsureComponent<ResourceTracker>();
            resourceTracker.prefabIdentifier = prefabIdentifier;
            resourceTracker.techType = TechType;
            resourceTracker.overrideTechType = TechType.Fragment;
            resourceTracker.rb = gameObject.GetComponent<Rigidbody>();
            resourceTracker.pickupable = pickupable;

            processedPrefab = gameObject;
            return processedPrefab;
        }

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> pistolFragment)
        {
            pistolFragment.Set(GetGameObject());
            yield break;
        }

        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromTexture(Main.assetBundle.LoadAsset<Texture2D>("Icon"));
        }
    }
}
#endif