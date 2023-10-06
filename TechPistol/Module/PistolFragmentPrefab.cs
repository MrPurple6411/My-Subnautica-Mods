#if !EDITOR
namespace TechPistol.Module;

using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using System.Collections.Generic;
using UnityEngine;
using UWE;
using static CraftData;

internal class PistolFragmentPrefab
{
    private static GameObject processedPrefab;
    private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");
    private static readonly int MetallicGlossMap = Shader.PropertyToID("_MetallicGlossMap");
    private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");
    public bool AddScannerEntry => true;
    public int FragmentsToScan => 3;
    public float TimeToScanFragment => 5f;
    public bool DestroyFragmentOnScan => true;
    public TechType RequiredForUnlock => Info.TechType;
    public bool UnlockedAtStart => false;

	public PrefabInfo Info { get; init; }

	public CustomPrefab Prefab { get; init; }

    public PistolFragmentPrefab()
    {
		Info = PrefabInfo.WithTechType("TechPistolFragment", "Tech Pistol", 
			"Incomplete or Broken fragment of an advanced pistol of unknown origins.")
			.WithIcon(ImageUtils.LoadSpriteFromTexture(Main.assetBundle.LoadAsset<Texture2D>("Icon")));
		Prefab = new CustomPrefab(Info);
		Prefab.SetGameObject(GetGameObject);
		Prefab.SetUnlock(RequiredForUnlock, FragmentsToScan)
			.WithAnalysisTech(null)
			.WithScannerEntry(TimeToScanFragment, true, null, true);
		Prefab.SetSpawns(entityInfo: EntityInfo, BiomesToSpawnIn);
	}

    public LootDistributionData.BiomeData[] BiomesToSpawnIn => GetBiomeDistribution();

    private static LootDistributionData.BiomeData[] GetBiomeDistribution()
    {
        var biomeDatas = new LootDistributionData.BiomeData[]
		{
#if SUBNAUTICA
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
#elif BELOWZERO
            new(){ biome = BiomeType.TwistyBridges_Ground, count = 1, probability = 0.2f },
            new(){ biome = BiomeType.TwistyBridges_Deep_Ground, count = 1, probability = 0.2f },
            new(){ biome = BiomeType.TwistyBridges_Cave_Ground, count = 1, probability = 0.2f },
            new(){ biome = BiomeType.TwistyBridges_Deep_ThermalVentArea_Ground, count = 1, probability = 0.2f },
            new(){ biome = BiomeType.GlacialBasin_BikeCrashSite, count = 1, probability = 0.2f },
            new(){ biome = BiomeType.GlacialBasin_Generic, count = 1, probability = 0.2f },
            new(){ biome = BiomeType.GlacialConnection_Ground, count = 1, probability = 0.2f },
            new(){ biome = BiomeType.Glacier_Generic, count = 1, probability = 0.2f },
            new(){ biome = BiomeType.LilyPads_Crevice_Ground, count = 1, probability = 0.2f },
            new(){ biome = BiomeType.LilyPads_Crevice_Grass, count = 1, probability = 0.2f },
            new(){ biome = BiomeType.LilyPads_Deep_Grass, count = 1, probability = 0.2f },
            new(){ biome = BiomeType.LilyPads_Deep_Ground, count = 1, probability = 0.2f },
#endif
        };
		return biomeDatas;
    }

    public WorldEntityInfo EntityInfo => new() { cellLevel = LargeWorldEntity.CellLevel.Near, classId = Info.ClassID, localScale = Vector3.one, prefabZUp = false, slotType = EntitySlot.Type.Small, techType = Info.TechType };

    public GameObject GetGameObject()
    {
        if (processedPrefab is not null) return processedPrefab;
        
        var prefab = Main.assetBundle.LoadAsset<GameObject>("TechPistol.prefab");
        var gameObject = EditorModifications.Instantiate(prefab, default, default, false);
        
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
        prefabIdentifier.ClassId = Info.ClassID;
        gameObject.GetComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
        gameObject.GetComponent<TechTag>().type = Info.TechType;

        var pickupable = gameObject.GetComponent<Pickupable>();
        pickupable.isPickupable = false;

        var resourceTracker = gameObject.EnsureComponent<ResourceTracker>();
        resourceTracker.prefabIdentifier = prefabIdentifier;
        resourceTracker.techType = Info.TechType;
        resourceTracker.overrideTechType = TechType.Fragment;
        resourceTracker.rb = gameObject.GetComponent<Rigidbody>();
        resourceTracker.pickupable = pickupable;

        processedPrefab = gameObject;
        return processedPrefab;
    }
}
#endif