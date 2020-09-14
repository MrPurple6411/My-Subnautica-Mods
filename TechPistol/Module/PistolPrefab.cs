using HarmonyLib;
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

namespace TechPistol.Module
{
    internal class PistolPrefab : Equipable
    {
		static HashSet<TechType> batteryChargerCompatibleTech => (HashSet<TechType>)Traverse.Create<BatteryCharger>().Field("compatibleTech").GetValue();
		static HashSet<TechType> powerCellChargerCompatibleTech => (HashSet<TechType>)Traverse.Create<PowerCellCharger>().Field("compatibleTech").GetValue();
		static List<TechType> compatibleTech => batteryChargerCompatibleTech.Concat(powerCellChargerCompatibleTech).ToList();


		public PistolPrefab() : base("techpistol", "Tech Pistol", "The Tech Pistol comes with a wide array of functionality including: Explosive Cannon, Laser Pistol, Target Health Detection and the Incredible Resizing Ray")
		{
		}

		public override EquipmentType EquipmentType => EquipmentType.Hand;
        public override Vector2int SizeInInventory => new Vector2int(2,2);
		public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Tools;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new string[] { "Personal", "Tools" };
        public override float CraftingTime => 5f;
        public override QuickSlotType QuickSlotType => base.QuickSlotType;

#if SUBNAUTICA_STABLE || BZ
        public override GameObject GetGameObject()
		{
			GameObject gameObject = Main.assetBundle.LoadAsset<GameObject>("TechPistol.prefab");
			MeshRenderer[] componentsInChildren = gameObject.transform.Find("HandGun").gameObject.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				if (meshRenderer.name.StartsWith("Gun"))
				{
					Texture emissionMap = meshRenderer.material.GetTexture("_EmissionMap");

					meshRenderer.material.shader = Shader.Find("MarmosetUBER");
					meshRenderer.material.EnableKeyword("_Glow");
					meshRenderer.material.SetTexture("_Illum", emissionMap);
					meshRenderer.material.SetColor("_EmissionColor", new Color(1f, 1f, 1f));
				}
			}

			SkyApplier skyApplier = gameObject.EnsureComponent<SkyApplier>();
			skyApplier.renderers = componentsInChildren;
			skyApplier.anchorSky = Skies.Auto;

			gameObject.EnsureComponent<PrefabIdentifier>().ClassId = base.ClassID;
			gameObject.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
			gameObject.EnsureComponent<Pickupable>().isPickupable = true;
			gameObject.EnsureComponent<TechTag>().type = base.TechType;

			WorldForces worldForces = gameObject.EnsureComponent<WorldForces>();
			Rigidbody useRigidbody = gameObject.EnsureComponent<Rigidbody>();
			worldForces.underwaterGravity = 0f;
			worldForces.useRigidbody = useRigidbody;
			EnergyMixin energyMixin = gameObject.EnsureComponent<EnergyMixin>();
			energyMixin.storageRoot = gameObject.transform.Find("HandGun/GunMain/BatteryRoot").gameObject.EnsureComponent<ChildObjectIdentifier>();
			energyMixin.allowBatteryReplacement = true;
			energyMixin.compatibleBatteries = compatibleTech;
			energyMixin.batteryModels = new EnergyMixin.BatteryModels[] { };

			foreach (TechType techType in compatibleTech)
			{
				energyMixin.batteryModels.AddItem(new EnergyMixin.BatteryModels
				{
					techType = techType,
					model = gameObject.transform.Find("HandGun/GunMain/BatteryRoot/Battery").gameObject
				});
			}

			RepulsionCannon component = CraftData.InstantiateFromPrefab(TechType.RepulsionCannon, false).GetComponent<RepulsionCannon>();
			StasisRifle component2 = CraftData.InstantiateFromPrefab(TechType.StasisRifle, false).GetComponent<StasisRifle>();
			PropulsionCannon component3 = CraftData.InstantiateFromPrefab(TechType.PropulsionCannon, false).GetComponent<PropulsionCannon>();
			Welder component4 = CraftData.InstantiateFromPrefab(TechType.Welder, false).GetComponent<Welder>();

			Pistol pistolBehaviour = gameObject.EnsureComponent<Pistol>();
			pistolBehaviour.repulsionCannonFireSound = component.shootSound;
			pistolBehaviour.stasisRifleFireSound = component2.fireSound;
			pistolBehaviour.stasisRifleEvent = component2.chargeBegin;
			pistolBehaviour.modeChangeSound = component3.shootSound;
			pistolBehaviour.laserShootSound = component4.weldSound;
			pistolBehaviour.mainCollider = gameObject.GetComponent<BoxCollider>();
			pistolBehaviour.ikAimRightArm = true;
			pistolBehaviour.useLeftAimTargetOnPlayer = true;

			GameObject.Destroy(component);
			GameObject.Destroy(component2);
			GameObject.Destroy(component3);
			GameObject.Destroy(component4);

			return gameObject;
		}
#endif

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> pistol)
		{
			GameObject gameObject = Main.assetBundle.LoadAsset<GameObject>("TechPistol.prefab");

			MeshRenderer[] componentsInChildren = gameObject.transform.Find("HandGun").gameObject.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				if (meshRenderer.name.StartsWith("Gun"))
				{
					Texture emissionMap = meshRenderer.material.GetTexture("_EmissionMap");
					meshRenderer.material.shader = Shader.Find("MarmosetUBER");
					meshRenderer.material.EnableKeyword("_Glow");
					meshRenderer.material.SetTexture("_Illum", emissionMap);
					meshRenderer.material.SetColor("_EmissionColor", new Color(1f, 1f, 1f));
				}
			}

			SkyApplier skyApplier = gameObject.EnsureComponent<SkyApplier>();
			skyApplier.renderers = componentsInChildren;
			skyApplier.anchorSky = Skies.Auto;

			gameObject.EnsureComponent<PrefabIdentifier>().ClassId = base.ClassID;
			gameObject.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
			gameObject.EnsureComponent<Pickupable>().isPickupable = true;
			gameObject.EnsureComponent<TechTag>().type = base.TechType;

			Rigidbody useRigidbody = gameObject.EnsureComponent<Rigidbody>();
			WorldForces worldForces = gameObject.EnsureComponent<WorldForces>();
			worldForces.underwaterGravity = 0f;
			worldForces.useRigidbody = useRigidbody;

			EnergyMixin energyMixin = gameObject.EnsureComponent<EnergyMixin>();
			energyMixin.storageRoot = gameObject.transform.Find("HandGun/GunMain/BatteryRoot").gameObject.EnsureComponent<ChildObjectIdentifier>();
			energyMixin.allowBatteryReplacement = true;
			energyMixin.compatibleBatteries = compatibleTech;
			energyMixin.batteryModels = new EnergyMixin.BatteryModels[] { };

			foreach (TechType techType in compatibleTech)
			{

				CoroutineTask<GameObject> batteryTask = CraftData.GetPrefabForTechTypeAsync(TechType.RepulsionCannon, false);
				yield return batteryTask;

				energyMixin.batteryModels.AddItem(new EnergyMixin.BatteryModels
				{
					techType = techType,
					model = gameObject.transform.Find("HandGun/GunMain/BatteryRoot/Battery").gameObject
				});

			}

			CoroutineTask<GameObject> task1 = CraftData.GetPrefabForTechTypeAsync(TechType.RepulsionCannon, false);
			yield return task1;
			GameObject gameObject1 = task1.GetResult();
			RepulsionCannon component = gameObject1.GetComponent<RepulsionCannon>();

			CoroutineTask<GameObject> task2 = CraftData.GetPrefabForTechTypeAsync(TechType.StasisRifle, false);
			yield return task2;
			GameObject gameObject2 = task2.GetResult();
			StasisRifle component2 = gameObject2.GetComponent<StasisRifle>();

			CoroutineTask<GameObject> task3 = CraftData.GetPrefabForTechTypeAsync(TechType.PropulsionCannon, false);
			yield return task3;
			GameObject gameObject3 = task3.GetResult();
			PropulsionCannon component3 = gameObject3.GetComponent<PropulsionCannon>();

			CoroutineTask<GameObject> task4 = CraftData.GetPrefabForTechTypeAsync(TechType.Welder, false);
			yield return task4;
			GameObject gameObject4 = task4.GetResult();
			Welder component4 = gameObject4.GetComponent<Welder>();


			Pistol pistolBehaviour = gameObject.EnsureComponent<Pistol>();
			pistolBehaviour.repulsionCannonFireSound = component.shootSound;
			pistolBehaviour.stasisRifleFireSound = component2.fireSound;
			pistolBehaviour.stasisRifleEvent = component2.chargeBegin;
			pistolBehaviour.modeChangeSound = component3.shootSound;
			pistolBehaviour.laserShootSound = component4.weldSound;
			pistolBehaviour.mainCollider = gameObject.GetComponent<BoxCollider>();
			pistolBehaviour.ikAimRightArm = true;
			pistolBehaviour.hasAnimations = false;
			pistolBehaviour.hasBashAnimation = false;
			pistolBehaviour.hasFirstUseAnimation = false;

			pistol.Set(gameObject);
			GameObject.Destroy(gameObject1);
			GameObject.Destroy(gameObject2);
			GameObject.Destroy(gameObject3);
			GameObject.Destroy(gameObject4);
			yield break;
		}

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData
			{
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(TechType.SeaTreaderPoop, 1),
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
