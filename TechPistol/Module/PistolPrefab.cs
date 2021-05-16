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
        private static List<TechType> modelsToMake = new List<TechType> { TechType.Battery, TechType.PrecursorIonBattery, TechType.PrecursorIonPowerCell, TechType.PowerCell };

        public PistolPrefab() : base("TechPistol", "Tech Pistol", "The Tech Pistol comes with a wide array of functionality including: Explosive Cannon, Laser Pistol, Target Health Detection and the Incredible Resizing Ray")
        {
        }

        private static GameObject processedPrefab;

        private static HashSet<TechType> BatteryChargerCompatibleTech => BatteryCharger.compatibleTech;
        private static HashSet<TechType> PowerCellChargerCompatibleTech => PowerCellCharger.compatibleTech;
        private static List<TechType> CompatibleTech => BatteryChargerCompatibleTech.Concat(PowerCellChargerCompatibleTech).ToList();

        public override EquipmentType EquipmentType => EquipmentType.Hand;
        public override Vector2int SizeInInventory => new Vector2int(2, 2);
        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Tools;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new string[] { "Personal", "Tools" };
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
                GameObject prefab = Main.assetBundle.LoadAsset<GameObject>("TechPistol.prefab");
                GameObject gameObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, false);

                MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
                foreach(MeshRenderer meshRenderer in componentsInChildren)
                {
                    if(meshRenderer.name.StartsWith("Gun") || meshRenderer.name.StartsWith("Target"))
                    {
                        Texture emissionMap = meshRenderer.material.GetTexture("_EmissionMap");
                        Texture specMap = meshRenderer.material.GetTexture("_MetallicGlossMap");

                        meshRenderer.material.shader = Shader.Find("MarmosetUBER");
                        meshRenderer.material.EnableKeyword("MARMO_EMISSION");
                        meshRenderer.material.EnableKeyword("MARMO_SPECMAP");
                        meshRenderer.material.SetTexture(ShaderPropertyID._Illum, emissionMap);
                        meshRenderer.material.SetTexture(ShaderPropertyID._SpecTex, specMap);
                        meshRenderer.material.SetColor("_GlowColor", new Color(1f, 1f, 1f));
                        meshRenderer.material.SetFloat(ShaderPropertyID._GlowStrength, 1f);
                        meshRenderer.material.SetFloat(ShaderPropertyID._GlowStrengthNight, 1f);
                    }
                }

                EnergyMixin energyMixin = gameObject.GetComponent<EnergyMixin>();
                List<EnergyMixin.BatteryModels> batteryModels = new List<EnergyMixin.BatteryModels>();
                foreach(TechType techType in modelsToMake)
                {
                    GameObject batteryprefab = CraftData.GetPrefabForTechType(techType, false);
                    GameObject model = GameObject.Instantiate(batteryprefab);
                    batteryprefab.SetActive(false);
                    model.SetActive(false);

                    GameObject.DestroyImmediate(model.GetComponentInChildren<WorldForces>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<Rigidbody>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<Battery>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<LargeWorldEntity>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<TechTag>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<EntityTag>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<Pickupable>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<Collider>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<SkyApplier>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<UniqueIdentifier>());

                    bool cellCheck = PowerCellCharger.compatibleTech.Contains(techType);

                    Vector3 position = cellCheck ? new Vector3(-0.11f, 0f, 0f) : new Vector3(-0.11f, 0.02f, 0f);
                    Vector3 scale = cellCheck ? new Vector3(0.15f, 0.15f, 0.15f) : new Vector3(0.3f, 0.3f, 0.3f);

                    model.transform.SetParent(gameObject.transform);
                    model.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
                    model.transform.localPosition = position;
                    model.transform.localEulerAngles = new Vector3(270f, 0f, 0f);
                    model.transform.localScale = scale;

                    batteryModels.Add(new EnergyMixin.BatteryModels
                    {
                        techType = techType,
                        model = model
                    });
                }

                energyMixin.compatibleBatteries = CompatibleTech;
                energyMixin.batteryModels = batteryModels.ToArray();

                gameObject.GetComponent<Rigidbody>().detectCollisions = false;

                foreach(UniqueIdentifier uniqueIdentifier in gameObject.GetComponentsInChildren<UniqueIdentifier>())
                    uniqueIdentifier.classId = this.ClassID;

                gameObject.GetComponent<TechTag>().type = base.TechType;
                gameObject.GetComponent<SkyApplier>().renderers = gameObject.GetComponentsInChildren<Renderer>(true);

                processedPrefab = gameObject;
                GameObject.DontDestroyOnLoad(processedPrefab);
                processedPrefab.EnsureComponent<SceneCleanerPreserve>();
            }
            var copy = GameObject.Instantiate(processedPrefab);
            GameObject.DestroyImmediate(copy.GetComponent<SceneCleanerPreserve>());
            return copy;
        }

#endif
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> pistol)
        {
            if(processedPrefab is null)
            {
                GameObject prefab = Main.assetBundle.LoadAsset<GameObject>("TechPistol.prefab");
                GameObject gameObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, false);
                GameObject.DestroyImmediate(prefab);

                MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
                foreach(MeshRenderer meshRenderer in componentsInChildren)
                {
                    if(meshRenderer.name.StartsWith("Gun") || meshRenderer.name.StartsWith("Target"))
                    {
                        Texture emissionMap = meshRenderer.material.GetTexture("_EmissionMap");
                        Texture specMap = meshRenderer.material.GetTexture("_MetallicGlossMap");

                        meshRenderer.material.shader = Shader.Find("MarmosetUBER");
                        meshRenderer.material.EnableKeyword("MARMO_EMISSION");
                        meshRenderer.material.EnableKeyword("MARMO_SPECMAP");
                        meshRenderer.material.SetTexture(ShaderPropertyID._Illum, emissionMap);
                        meshRenderer.material.SetTexture(ShaderPropertyID._SpecTex, specMap);
                        meshRenderer.material.SetColor("_GlowColor", new Color(1f, 1f, 1f));
                        meshRenderer.material.SetFloat(ShaderPropertyID._GlowStrength, 1f);
                        meshRenderer.material.SetFloat(ShaderPropertyID._GlowStrengthNight, 1f);
                    }
                }

                EnergyMixin energyMixin = gameObject.GetComponent<EnergyMixin>();
                List<EnergyMixin.BatteryModels> batteryModels = new List<EnergyMixin.BatteryModels>();

                foreach(TechType techType in modelsToMake)
                {
                    CoroutineTask<GameObject> batteryTask = CraftData.GetPrefabForTechTypeAsync(techType, false);
                    yield return batteryTask;

                    GameObject batteryprefab = batteryTask.GetResult();
                    GameObject model = GameObject.Instantiate(batteryprefab);
                    batteryprefab.SetActive(false);
                    model.SetActive(false);

                    GameObject.DestroyImmediate(model.GetComponentInChildren<WorldForces>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<Rigidbody>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<Battery>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<LargeWorldEntity>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<TechTag>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<EntityTag>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<Pickupable>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<Collider>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<SkyApplier>());
                    GameObject.DestroyImmediate(model.GetComponentInChildren<UniqueIdentifier>());

                    bool cellCheck = PowerCellCharger.compatibleTech.Contains(techType);

                    Vector3 position = cellCheck ? new Vector3(-0.11f, 0f, 0f) : new Vector3(-0.11f, 0.02f, 0f);
                    Vector3 scale = cellCheck ? new Vector3(0.15f, 0.15f, 0.15f) : new Vector3(0.3f, 0.3f, 0.3f);

                    model.transform.SetParent(gameObject.transform);
                    model.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
                    model.transform.localPosition = position;
                    model.transform.localEulerAngles = new Vector3(270f, 0f, 0f);
                    model.transform.localScale = scale;

                    batteryModels.Add(new EnergyMixin.BatteryModels
                    {
                        techType = techType,
                        model = model
                    });
                }

                energyMixin.compatibleBatteries = CompatibleTech;
                energyMixin.batteryModels = batteryModels.ToArray();

                gameObject.GetComponent<Rigidbody>().detectCollisions = false;
                foreach(UniqueIdentifier uniqueIdentifier in gameObject.GetComponentsInChildren<UniqueIdentifier>())
                    uniqueIdentifier.classId = this.ClassID;
                gameObject.GetComponent<TechTag>().type = base.TechType;
                gameObject.GetComponent<SkyApplier>().renderers = gameObject.GetComponentsInChildren<Renderer>(true);

                processedPrefab = gameObject;
                GameObject.DontDestroyOnLoad(processedPrefab);
                processedPrefab.EnsureComponent<SceneCleanerPreserve>();
            }

            var copy = GameObject.Instantiate(processedPrefab);
            GameObject.DestroyImmediate(copy.GetComponent<SceneCleanerPreserve>());
            pistol.Set(copy);
            yield break;
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(TechType.TitaniumIngot, 1),
                    new Ingredient(TechType.Lubricant, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.EnameledGlass, 1)
                }
            };
        }

        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromTexture(Main.assetBundle.LoadAsset<Texture2D>("Icon"));
        }
    }
}
