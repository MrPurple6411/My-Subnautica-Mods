#if !EDITOR
namespace TechPistol.Module;

using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CraftData;

internal class PistolPrefab
{
	private const string FriendlyName = "Tech Pistol";
	private static readonly List<TechType> modelsToMake = new() { TechType.Battery, TechType.PrecursorIonBattery, TechType.PrecursorIonPowerCell, TechType.PowerCell };

	public PrefabInfo Info { get; init; }
	public CustomPrefab Prefab { get; init; }

    public PistolPrefab() 
    {
		Info = PrefabInfo.WithTechType("TechPistol", FriendlyName, "The Tech Pistol comes with a wide array of functionality including: Explosive Cannon, Laser Pistol, Target Health Detection and the Incredible Resizing Ray")
			.WithIcon(ImageUtils.LoadSpriteFromTexture(Main.assetBundle.LoadAsset<Texture2D>("Icon"))).WithSizeInInventory(SizeInInventory);

		Prefab = new CustomPrefab(Info);
		Prefab.SetUnlock(RequiredForUnlock)
			.WithAnalysisTech(null, null, null)
			.WithPdaGroupCategory(GroupForPDA, CategoryForPDA);
		Prefab.SetEquipment(EquipmentType)
			.WithQuickSlotType(QuickSlotType);
		Prefab.SetRecipe(GetBlueprintRecipe())
			.WithCraftingTime(CraftingTime)
			.WithFabricatorType(FabricatorType)
			.WithStepsToFabricatorTab(StepsToFabricatorTab);

		Prefab.SetGameObject(GetGameObjectAsync);

    }

    private static GameObject processedPrefab;
    private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");
    private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");
    private static readonly int Fresnel = Shader.PropertyToID("_Fresnel");

    private static HashSet<TechType> BatteryChargerCompatibleTech => BatteryCharger.compatibleTech;
    private static HashSet<TechType> PowerCellChargerCompatibleTech => PowerCellCharger.compatibleTech;
    private static List<TechType> CompatibleTech => BatteryChargerCompatibleTech.Concat(PowerCellChargerCompatibleTech).ToList();

    public EquipmentType EquipmentType => EquipmentType.Hand;
    public Vector2int SizeInInventory => new(2, 2);
    public TechGroup GroupForPDA => TechGroup.Personal;
    public TechCategory CategoryForPDA => TechCategory.Tools;
    public CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
    public string[] StepsToFabricatorTab => new[] { "Personal", "Tools" };
    public float CraftingTime => 5f;
    public QuickSlotType QuickSlotType => QuickSlotType.Selectable;

    public TechType RequiredForUnlock => Main.PistolFragment.Info.TechType;

    public List<TechType> CompoundTechsForUnlock => GetUnlocks();

    private List<TechType> GetUnlocks()
    {
        var list = new List<TechType>();

        foreach (var ingredient in this.GetBlueprintRecipe().Ingredients)
        {
            list.Add(ingredient.techType);
        }

        return list;
    }

    public IEnumerator GetGameObjectAsync(IOut<GameObject> pistol)
    {
        if(processedPrefab is null)
        {
            var prefab = Main.assetBundle.LoadAsset<GameObject>("TechPistol.prefab");
            var gameObject = EditorModifications.Instantiate(prefab, Vector3.zero, Quaternion.identity, false);

            var componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
            Shader marmo = Shader.Find("MarmosetUBER");
            foreach(var meshRenderer in componentsInChildren)
            {
                if (!meshRenderer.name.StartsWith("Gun") && !meshRenderer.name.StartsWith("Target")) continue;
                var material = meshRenderer.material;

                material.shader = marmo;
                material.EnableKeyword("MARMO_EMISSION");
                material.EnableKeyword("MARMO_SPECMAP");
            }

            var energyMixin = gameObject.GetComponent<EnergyMixin>();
            var batteryModels = new List<EnergyMixin.BatteryModels>();

            foreach(var techType in modelsToMake)
            {
                var cellCheck = PowerCellCharger.compatibleTech.Contains(techType);

                var position = cellCheck ? new Vector3(-0.11f, 0f, 0f) : new Vector3(-0.11f, 0.02f, 0f);
                var scale = cellCheck ? new Vector3(0.15f, 0.15f, 0.15f) : new Vector3(0.3f, 0.3f, 0.3f);
                
                var batteryTask = CraftData.GetPrefabForTechTypeAsync(techType, false);
                yield return batteryTask;

                var batteryPrefab = batteryTask.GetResult();
                var model = EditorModifications.Instantiate(batteryPrefab, gameObject.transform, position, gameObject.transform.rotation, scale, false);
                batteryPrefab.SetActive(false);

                Object.DestroyImmediate(model.GetComponentInChildren<WorldForces>());
                Object.DestroyImmediate(model.GetComponentInChildren<Rigidbody>());
                Object.DestroyImmediate(model.GetComponentInChildren<Battery>());
                Object.DestroyImmediate(model.GetComponentInChildren<LargeWorldEntity>());
                Object.DestroyImmediate(model.GetComponentInChildren<TechTag>());
                Object.DestroyImmediate(model.GetComponentInChildren<EntityTag>());
                Object.DestroyImmediate(model.GetComponentInChildren<Pickupable>());
                Object.DestroyImmediate(model.GetComponentInChildren<Collider>());
                Object.DestroyImmediate(model.GetComponentInChildren<SkyApplier>());
                Object.DestroyImmediate(model.GetComponentInChildren<UniqueIdentifier>());

                model.transform.localEulerAngles = new Vector3(270f, 0f, 0f);

                batteryModels.Add(new EnergyMixin.BatteryModels
                {
                    techType = techType,
                    model = model
                });
            }

            energyMixin.compatibleBatteries = CompatibleTech;
            energyMixin.batteryModels = batteryModels.ToArray();

            gameObject.GetComponent<Rigidbody>().detectCollisions = false;
            foreach(var uniqueIdentifier in gameObject.GetComponentsInChildren<UniqueIdentifier>())
                uniqueIdentifier.classId = Info.ClassID;
            gameObject.GetComponent<TechTag>().type = Info.TechType;
            gameObject.GetComponent<SkyApplier>().renderers = gameObject.GetComponentsInChildren<Renderer>(true);

            processedPrefab = gameObject;
            Object.DontDestroyOnLoad(processedPrefab);
            processedPrefab.EnsureComponent<SceneCleanerPreserve>();
			processedPrefab.SetActive(false);
        }

        var copy = Object.Instantiate(processedPrefab);
        Object.DestroyImmediate(copy.GetComponent<SceneCleanerPreserve>());
		copy.SetActive(false);
        pistol.Set(copy);
    }

    protected RecipeData GetBlueprintRecipe()
    {
        return new()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>
            {
                new(TechType.TitaniumIngot, 1),
                new(TechType.Lubricant, 1),
                new(TechType.AdvancedWiringKit, 1),
                new(TechType.EnameledGlass, 1)
            }
        };
    }
}
#endif