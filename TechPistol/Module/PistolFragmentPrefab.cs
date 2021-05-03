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

        public PistolFragmentPrefab() : base(
            "TechPistolFragment",
            "Damaged Pistol Fragment",
            "Incomplete or Broken fragment of an advanced pistol of unknown origins."
            )
        {
        }

        public override List<LootDistributionData.BiomeData> BiomesToSpawnIn => GetBiomeDistribution();


        private List<LootDistributionData.BiomeData> GetBiomeDistribution()
        {
#if SN1
            List<LootDistributionData.BiomeData> biomeDatas = new List<LootDistributionData.BiomeData>()
            {
                new LootDistributionData.BiomeData(){ biome = BiomeType.DeepGrandReef_AbandonedBase_Interior, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.DeepGrandReef_AbandonedBase_Exterior, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.FloatingIslands_AbandonedBase_Inside, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.FloatingIslands_AbandonedBase_Outside, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.JellyShroomCaves_AbandonedBase_Inside, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.JellyShroomCaves_AbandonedBase_Outside, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.JellyshroomCaves_CaveFloor, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.JellyshroomCaves_CaveSand, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.LostRiverJunction_Ground, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.LostRiverCorridor_Ground, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.LostRiverCorridor_LakeFloor, count = 1, probability = 0.2f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.LostRiverJunction_LakeFloor, count = 1, probability = 0.2f },

            };
#elif BZ
            List<LootDistributionData.BiomeData> biomeDatas = new List<LootDistributionData.BiomeData>()
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

        public override WorldEntityInfo EntityInfo => new WorldEntityInfo() { cellLevel = LargeWorldEntity.CellLevel.Medium, classId = ClassID, localScale = Vector3.one, prefabZUp = false, slotType = EntitySlot.Type.Small, techType = TechType };

        public override GameObject GetGameObject()
        {
            if(processedPrefab is null)
            {
                GameObject prefab = Main.assetBundle.LoadAsset<GameObject>("TechPistol.prefab");
                GameObject gameObject = GameObject.Instantiate(prefab);
                gameObject.SetActive(false);
                prefab.SetActive(false);

                gameObject.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                gameObject.transform.localPosition += Vector3.up * 2;

                Renderer[] componentsInChildren = gameObject.transform.Find("HandGun").gameObject.GetComponentsInChildren<Renderer>();
                foreach(Renderer renderer in componentsInChildren)
                {
                    if(renderer.name.StartsWith("Gun") || renderer.name.StartsWith("Target"))
                    {
                        Texture emissionMap = renderer.material.GetTexture("_EmissionMap");
                        Texture specMap = renderer.material.GetTexture("_MetallicGlossMap");

                        renderer.material.shader = Shader.Find("MarmosetUBER");
                        renderer.material.EnableKeyword("MARMO_EMISSION");
                        renderer.material.EnableKeyword("MARMO_SPECMAP");
                        renderer.material.SetTexture(ShaderPropertyID._Illum, emissionMap);
                        renderer.material.SetTexture(ShaderPropertyID._SpecTex, specMap);
                        renderer.material.SetColor("_GlowColor", new Color(1f, 1f, 1f));
                        renderer.material.SetFloat(ShaderPropertyID._GlowStrength, 1f);
                        renderer.material.SetFloat(ShaderPropertyID._GlowStrengthNight, 1f);
                    }
                }

                GameObject.Destroy(gameObject.transform.Find(PistolBehaviour.GunMain + "/ModeChange")?.gameObject);
                GameObject.Destroy(gameObject.transform.Find(PistolBehaviour.Point)?.gameObject);
                GameObject.Destroy(gameObject.GetComponent<PistolBehaviour>());
                GameObject.Destroy(gameObject.GetComponent<EnergyMixin>());
                GameObject.Destroy(gameObject.GetComponent<VFXFabricating>());

                PrefabIdentifier prefabIdentifier = gameObject.GetComponent<PrefabIdentifier>();
                prefabIdentifier.ClassId = ClassID;
                gameObject.GetComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.VeryFar;
                gameObject.GetComponent<TechTag>().type = TechType;

                Pickupable pickupable = gameObject.GetComponent<Pickupable>();
                pickupable.isPickupable = false;

                ResourceTracker resourceTracker = gameObject.EnsureComponent<ResourceTracker>();
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
