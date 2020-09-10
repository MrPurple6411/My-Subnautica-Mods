using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if SN1
using Data = SMLHelper.V2.Crafting.TechData;
using Sprite = Atlas.Sprite;
#elif BZ
using Data = SMLHelper.V2.Crafting.RecipeData;
#endif

namespace TechPistol.Module
{
    internal class PistolPrefab : Equipable
    {
        public PistolPrefab() : base("techpistol", "Tech Pistol", "The Tech Pistol comes with a wide array of functionality including: Explosive Cannon, Laser Pistol, Target Health Detection and the Incredible Resizing Ray")
        {

        }

        public override EquipmentType EquipmentType => EquipmentType.Hand;

        public override Vector2int SizeInInventory => new Vector2int(2,2);

        public override TechGroup GroupForPDA => TechGroup.Personal;

        public override TechCategory CategoryForPDA => TechCategory.Tools;

        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;

        public override string[] StepsToFabricatorTab => new string[] { "Personal", "Tools", "techpistol" };

        public override float CraftingTime => 5f;

        public override QuickSlotType QuickSlotType => base.QuickSlotType;

#if SUBNAUTICA_STABLE || BZ
        public override GameObject GetGameObject()
		{
			GameObject result;
			try
			{
				GameObject gameObject = Main.assetBundle.LoadAsset<GameObject>("techpistol.prefab");
				MeshRenderer[] componentsInChildren = Radical.FindChild(gameObject, "HandGun").GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer meshRenderer in componentsInChildren)
				{
					meshRenderer.material.shader = Shader.Find("MarmosetUBER");
					meshRenderer.material.SetColor("_Emission", new Color(1f, 1f, 1f));
				}
				gameObject.transform.Find("Cannonmode/shoot/shoo").gameObject.EnsureComponent<ExplosionBehaviour>();
				gameObject.EnsureComponent<PrefabIdentifier>().ClassId = base.ClassID;
				gameObject.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
				gameObject.EnsureComponent<Pickupable>().isPickupable = true;
				gameObject.EnsureComponent<TechTag>().type = base.TechType;

				WorldForces worldForces = gameObject.EnsureComponent<WorldForces>();
				Rigidbody useRigidbody = gameObject.EnsureComponent<Rigidbody>();
				worldForces.underwaterGravity = 0f;
				worldForces.useRigidbody = useRigidbody;
				EnergyMixin energyMixin = gameObject.EnsureComponent<EnergyMixin>();
				energyMixin.storageRoot = Radical.FindChild(gameObject, "BatteryRoot").EnsureComponent<ChildObjectIdentifier>();
				energyMixin.allowBatteryReplacement = true;
				energyMixin.compatibleBatteries = new List<TechType>
				{
					TechType.PrecursorIonBattery,
					TechType.Battery,
					TechType.PrecursorIonPowerCell,
					TechType.PowerCell
				};
				energyMixin.batteryModels = new EnergyMixin.BatteryModels[]
				{
					new EnergyMixin.BatteryModels
					{
						techType = TechType.PrecursorIonPowerCell,
						model = gameObject.transform.Find("BatteryRoot/PrecursorIonPowerCell").gameObject
					},
					new EnergyMixin.BatteryModels
					{
						techType = TechType.Battery,
						model = gameObject.transform.Find("BatteryRoot/Battery").gameObject
					},
					new EnergyMixin.BatteryModels
					{
						techType = TechType.PrecursorIonBattery,
						model = gameObject.transform.Find("BatteryRoot/PrecursorIonBattery").gameObject
					},
					new EnergyMixin.BatteryModels
					{
						techType = TechType.PowerCell,
						model = gameObject.transform.Find("BatteryRoot/PowerCell").gameObject
					}
				};
				Pistol pistol = gameObject.EnsureComponent<Pistol>();
				RepulsionCannon component = CraftData.InstantiateFromPrefab(TechType.RepulsionCannon, false).GetComponent<RepulsionCannon>();
				StasisRifle component2 = CraftData.InstantiateFromPrefab(TechType.StasisRifle, false).GetComponent<StasisRifle>();
				PropulsionCannon component3 = CraftData.InstantiateFromPrefab(TechType.PropulsionCannon, false).GetComponent<PropulsionCannon>();
				Welder component4 = CraftData.InstantiateFromPrefab(TechType.Welder, false).GetComponent<Welder>();
				VFXFabricating vfxfabricating = Radical.FindChild(gameObject, "HandGun").EnsureComponent<VFXFabricating>();
				vfxfabricating.localMinY = -3f;
				vfxfabricating.localMaxY = 3f;
				vfxfabricating.posOffset = new Vector3(0f, 0f, 0f);
				vfxfabricating.eulerOffset = new Vector3(0f, 90f, -90f);
				vfxfabricating.scaleFactor = 1f;
				pistol.shoot1 = component.shootSound;
				pistol.shoot2 = component2.fireSound;
				pistol.xulikai = component2.chargeBegin;
				pistol.modechang = component3.shootSound;
				pistol.laseroopS = component4.weldSound;
				pistol.mainCollider = gameObject.GetComponent<BoxCollider>();
				pistol.ikAimRightArm = true;
				pistol.useLeftAimTargetOnPlayer = true;
				UnityEngine.Object.Destroy(component2);
				UnityEngine.Object.Destroy(component3);
				UnityEngine.Object.Destroy(component);
				UnityEngine.Object.Destroy(component4);
				result = gameObject;
			}
			catch
			{
				result = new GameObject();
			}
			return result;
		}
#endif

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
		{
			GameObject prefab;
			Pistol pistol;

			try
			{
				prefab = Main.assetBundle.LoadAsset<GameObject>("techpistol.prefab");
				MeshRenderer[] componentsInChildren = Radical.FindChild(prefab, "HandGun").GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer meshRenderer in componentsInChildren)
				{
					meshRenderer.material.shader = Shader.Find("MarmosetUBER");
					meshRenderer.material.SetColor("_Emission", new Color(1f, 1f, 1f));
				}
				prefab.transform.Find("Cannonmode/shoot/shoo").gameObject.EnsureComponent<ExplosionBehaviour>();
				prefab.EnsureComponent<PrefabIdentifier>().ClassId = base.ClassID;
				prefab.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
				prefab.EnsureComponent<Pickupable>().isPickupable = true;
				prefab.EnsureComponent<TechTag>().type = base.TechType;

				WorldForces worldForces = prefab.EnsureComponent<WorldForces>();
				Rigidbody useRigidbody = prefab.EnsureComponent<Rigidbody>();
				worldForces.underwaterGravity = 0f;
				worldForces.useRigidbody = useRigidbody;
				EnergyMixin energyMixin = prefab.EnsureComponent<EnergyMixin>();
				energyMixin.storageRoot = Radical.FindChild(prefab, "BatteryRoot").EnsureComponent<ChildObjectIdentifier>();
				energyMixin.allowBatteryReplacement = true;
				energyMixin.compatibleBatteries = new List<TechType>
				{
					TechType.PrecursorIonBattery,
					TechType.Battery,
					TechType.PrecursorIonPowerCell,
					TechType.PowerCell
				};
				energyMixin.batteryModels = new EnergyMixin.BatteryModels[]
				{
					new EnergyMixin.BatteryModels
					{
						techType = TechType.PrecursorIonPowerCell,
						model = prefab.transform.Find("BatteryRoot/PrecursorIonPowerCell").gameObject
					},
					new EnergyMixin.BatteryModels
					{
						techType = TechType.Battery,
						model = prefab.transform.Find("BatteryRoot/Battery").gameObject
					},
					new EnergyMixin.BatteryModels
					{
						techType = TechType.PrecursorIonBattery,
						model = prefab.transform.Find("BatteryRoot/PrecursorIonBattery").gameObject
					},
					new EnergyMixin.BatteryModels
					{
						techType = TechType.PowerCell,
						model = prefab.transform.Find("BatteryRoot/PowerCell").gameObject
					}
				};
				pistol = prefab.EnsureComponent<Pistol>();
			}
			catch
			{
				gameObject.Set(new GameObject());
				yield break;
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

			try
			{
				VFXFabricating vfxfabricating = Radical.FindChild(prefab, "HandGun").EnsureComponent<VFXFabricating>();
				vfxfabricating.localMinY = -3f;
				vfxfabricating.localMaxY = 3f;
				vfxfabricating.posOffset = new Vector3(0f, 0f, 0f);
				vfxfabricating.eulerOffset = new Vector3(0f, 90f, -90f);
				vfxfabricating.scaleFactor = 1f;
				pistol.shoot1 = component.shootSound;
				pistol.shoot2 = component2.fireSound;
				pistol.xulikai = component2.chargeBegin;
				pistol.modechang = component3.shootSound;
				pistol.laseroopS = component4.weldSound;
				pistol.mainCollider = prefab.GetComponent<BoxCollider>();
				pistol.ikAimRightArm = true;
				pistol.useLeftAimTargetOnPlayer = true;
				UnityEngine.Object.Destroy(component2);
				UnityEngine.Object.Destroy(component3);
				UnityEngine.Object.Destroy(component);
				UnityEngine.Object.Destroy(component4);

				gameObject.Set(prefab);
				yield break;
			}
			catch
			{
				gameObject.Set(new GameObject());
				yield break;
			}
		}

        protected override Data GetBlueprintRecipe()
        {
            return new Data
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(TechType.SeaTreaderPoop, 1),
                    new Ingredient(TechType.TitaniumIngot, 2),
                    new Ingredient(TechType.Lubricant, 1),
                    new Ingredient(TechType.EnameledGlass, 3)
                }
            };
        }

        protected override Sprite GetItemSprite()
        {
#if SN1
            return new Sprite(Main.assetBundle.LoadAsset<UnityEngine.Sprite>("Icon"), false);
#elif BZ
            return Main.assetBundle.LoadAsset<Sprite>("Icon");
#endif
        }
    }
}
