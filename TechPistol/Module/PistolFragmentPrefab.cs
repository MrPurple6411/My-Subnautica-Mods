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

        public override WorldEntityInfo EntityInfo => new() { cellLevel = LargeWorldEntity.CellLevel.Medium, classId = ClassID, localScale = Vector3.one, prefabZUp = false, slotType = EntitySlot.Type.Small, techType = TechType };

        public override GameObject GetGameObject()
        {
            if(processedPrefab is null)
            {
                var prefab = Main.assetBundle.LoadAsset<GameObject>("TechPistol.prefab");
                var gameObject = Object.Instantiate(prefab);
                gameObject.SetActive(false);
                prefab.SetActive(false);

                gameObject.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                gameObject.transform.localPosition += Vector3.up * 2;

                var componentsInChildren = gameObject.transform.Find("HandGun").gameObject.GetComponentsInChildren<Renderer>();
                foreach(var renderer in componentsInChildren)
                {
                    if(renderer.name.StartsWith("Gun") || renderer.name.StartsWith("Target"))
                    {
                        var emissionMap = renderer.material.GetTexture(EmissionMap);
                        var specMap = renderer.material.GetTexture(MetallicGlossMap);

                        renderer.material.shader = Shader.Find("MarmosetUBER");
                        renderer.material.EnableKeyword("MARMO_EMISSION");
                        renderer.material.EnableKeyword("MARMO_SPECMAP");
                        renderer.material.SetTexture(ShaderPropertyID._Illum, emissionMap);
                        renderer.material.SetTexture(ShaderPropertyID._SpecTex, specMap);
                        renderer.material.SetColor(GlowColor, new Color(1f, 1f, 1f));
                        renderer.material.SetFloat(ShaderPropertyID._GlowStrength, 1f);
                        renderer.material.SetFloat(ShaderPropertyID._GlowStrengthNight, 1f);
                    }
                }

                Object.Destroy(gameObject.transform.Find(PistolBehaviour.GunMain + "/ModeChange")?.gameObject);
                Object.Destroy(gameObject.transform.Find(PistolBehaviour.Point)?.gameObject);
                Object.Destroy(gameObject.GetComponent<PistolBehaviour>());
                Object.Destroy(gameObject.GetComponent<EnergyMixin>());
                Object.Destroy(gameObject.GetComponent<VFXFabricating>());

                var prefabIdentifier = gameObject.GetComponent<PrefabIdentifier>();
                prefabIdentifier.ClassId = ClassID;
                gameObject.GetComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.VeryFar;
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
            }
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
