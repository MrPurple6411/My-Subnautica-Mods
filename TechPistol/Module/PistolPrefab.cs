#if !EDITOR
namespace TechPistol.Module
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Utility;

    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

#if SN1
    using RecipeData = SMLHelper.V2.Crafting.TechData;
    using Sprite = Atlas.Sprite;
#endif

    internal class PistolPrefab: Equipable
    {
        private static readonly List<TechType> modelsToMake = new() { TechType.Battery, TechType.PrecursorIonBattery, TechType.PrecursorIonPowerCell, TechType.PowerCell };

        public PistolPrefab() : base("TechPistol", "Tech Pistol", "The Tech Pistol comes with a wide array of functionality including: Explosive Cannon, Laser Pistol, Target Health Detection and the Incredible Resizing Ray")
        {
        }

        private static GameObject processedPrefab;
        private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");
        private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");
        private static readonly int Fresnel = Shader.PropertyToID("_Fresnel");

        private static HashSet<TechType> BatteryChargerCompatibleTech => BatteryCharger.compatibleTech;
        private static HashSet<TechType> PowerCellChargerCompatibleTech => PowerCellCharger.compatibleTech;
        private static List<TechType> CompatibleTech => BatteryChargerCompatibleTech.Concat(PowerCellChargerCompatibleTech).ToList();

        public override EquipmentType EquipmentType => EquipmentType.Hand;
        public override Vector2int SizeInInventory => new(2, 2);
        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Tools;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new[] { "Personal", "Tools" };
        public override float CraftingTime => 5f;
        public override QuickSlotType QuickSlotType => QuickSlotType.Selectable;
        public override TechType RequiredForUnlock => Main.PistolFragment.TechType;
        public override string DiscoverMessage => $"{FriendlyName} Unlocked!";
        public override bool AddScannerEntry => true;
        public override int FragmentsToScan => 5;
        public override float TimeToScanFragment => 5f;
        public override bool DestroyFragmentOnScan => true;

#if SUBNAUTICA_STABLE

        public override GameObject GetGameObject()
        {
            if(processedPrefab is null)
            {
                var prefab = Main.assetBundle.LoadAsset<GameObject>("TechPistol.prefab");
                var gameObject = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, false);

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

                    var batteryPrefab = CraftData.GetPrefabForTechType(techType, false);
                    var model = Object.Instantiate(batteryPrefab, gameObject.transform, position, gameObject.transform.rotation, scale, false);
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
                    uniqueIdentifier.classId = ClassID;

                gameObject.GetComponent<TechTag>().type = TechType;
                gameObject.GetComponent<SkyApplier>().renderers = gameObject.GetComponentsInChildren<Renderer>(true);
                
                processedPrefab = gameObject;
            }
            return processedPrefab;
        }

#endif
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> pistol)
        {
            if(processedPrefab is null)
            {
                var prefab = Main.assetBundle.LoadAsset<GameObject>("TechPistol.prefab");
                var gameObject = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, false);

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
                    var model = Object.Instantiate(batteryPrefab, gameObject.transform, position, gameObject.transform.rotation, scale, false);
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
                    uniqueIdentifier.classId = ClassID;
                gameObject.GetComponent<TechTag>().type = TechType;
                gameObject.GetComponent<SkyApplier>().renderers = gameObject.GetComponentsInChildren<Renderer>(true);

                processedPrefab = gameObject;
                Object.DontDestroyOnLoad(processedPrefab);
                processedPrefab.EnsureComponent<SceneCleanerPreserve>();
            }

            var copy = Object.Instantiate(processedPrefab);
            Object.DestroyImmediate(copy.GetComponent<SceneCleanerPreserve>());
            pistol.Set(copy);
        }

        protected override RecipeData GetBlueprintRecipe()
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

        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromTexture(Main.assetBundle.LoadAsset<Texture2D>("Icon"));
        }
    }
}
#endif