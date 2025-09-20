namespace BuildingTweaks.Patches;

using HarmonyLib;
using UnityEngine;
using Nautilus.Handlers;
using System.Linq;
using System;

[HarmonyPatch(typeof(Player), nameof(Player.Update))]
public static class Player_Update_Patch
{
	private static bool builderModuleChecked = false;
	private static TechType builderModuleTechType = TechType.None;

	[HarmonyPostfix]
	public static void Postfix(Player __instance)
	{
		try
		{
			EnsurePositioning(__instance);

			// Don't run if the console is open
			if (DevConsole.instance != null && DevConsole.instance.state)
				return;

			bool builderCheck = false;

			if (!builderModuleChecked)
			{
				builderModuleChecked = true;
				TechTypeExtensions.FromString("BuilderModule", out builderModuleTechType, true);
			}
			
			Vehicle vehicle = __instance.GetVehicle();
			if (vehicle != null)
			{
				Pickupable module = vehicle?.GetSlotItem(vehicle.GetActiveSlotID())?.item;
				builderCheck = module != null && module.GetTechType() == builderModuleTechType;
			}
			else
			{
				PlayerTool heldTool = Inventory.main.GetHeldTool();
			 	builderCheck = heldTool != null && heldTool.pickupable != null && heldTool.pickupable.GetTechType() == TechType.Builder;
			}

			if (builderCheck)
			{
				ProcessMSG($"Attach as target override = {Main.Config.AttachToTarget}", true);
				ProcessMSG($"Full Override = {Main.Config.FullOverride}", true);

				if (Input.GetKeyDown(Main.Config.AttachToTargetToggle))
				{
					ProcessMSG($"Attach as target override = {Main.Config.AttachToTarget}", false);
					Main.Config.AttachToTarget = !Main.Config.AttachToTarget;
					ProcessMSG($"Attach as target override = {Main.Config.AttachToTarget}", true);
				}

				if (Input.GetKeyDown(Main.Config.FullOverrideToggle))
				{
					ProcessMSG($"Full Override = {Main.Config.FullOverride}", false);
					Main.Config.FullOverride = !Main.Config.FullOverride;
					if (Builder.prefab != null && !Builder.canPlace)
					{
						var value = Main.Config.FullOverride ? Builder.placeColorAllow : Builder.placeColorDeny;
						var components = Builder.ghostModel.GetComponents<IBuilderGhostModel>();
						foreach (var builderGhostModel in components)
							builderGhostModel.UpdateGhostModelColor(true, ref value);
						Builder.ghostStructureMaterial.SetColor(ShaderPropertyID._Tint, value);
					}
					ProcessMSG($"Full Override = {Main.Config.FullOverride}", true);
				}
			}
			else
			{
				ClearMsgs();
			}
		}
		catch (Exception ex)
		{
			Main.Logger.LogError($"Error in Player_Update_Patch : {ex.Message}");
			ProcessMSG($"Error in Player_Update_Patch : {ex.Message}", true);
		}
	}

	private static void EnsurePositioning(Player player)
	{
#if SUBNAUTICA

			var currentSubRoot = player.GetCurrentSub();
			if (currentSubRoot != null && currentSubRoot is BaseRoot && player.playerController.velocity.y < -20f)
			{
				var componentInChildren = currentSubRoot.gameObject.GetComponentInChildren<RespawnPoint>();
				if (componentInChildren)
				{
					player.SetPosition(componentInChildren.GetSpawnPosition());
					return;
				}
			}

			var escapePod = player.currentEscapePod;
			if (escapePod == null || !(player.playerController.velocity.y < -20f)) return;
			var transform = escapePod.playerSpawn.transform;
			player.SetPosition(transform.position, transform.rotation);
#elif BELOWZERO
		var interiorSpace = player.currentInterior;
		if (interiorSpace == null || !(player.playerController.velocity.y < -20f)) return;
		var respawnPoint = interiorSpace.GetRespawnPoint();
		if (!respawnPoint) return;
		player.SetPosition(respawnPoint.GetSpawnPosition());
#endif

		var waterPark = player.currentWaterPark;
			if (waterPark != null && waterPark.GetComponentInParent<Creature>() != null)
			{
				var vector3 = player.currentWaterPark.transform.position;
				player.SetPosition(vector3);

				// Use device getter per game
#if BELOWZERO
				var device = GameInput.GetPrimaryDevice();
#else
				var device = GameInput.PrimaryDevice;
#endif
				var msg3 = $"Press {GameInput.GetBinding(device, GameInput.Button.Exit, GameInput.BindingSet.Primary)} to exit waterpark if you cant reach the exit.";

				if (!GameInput.GetButtonDown(GameInput.Button.Exit))
				{
					ProcessMSG(msg3, true);
					return;
				}

				Collider[] hitColliders = { };
				Physics.OverlapSphereNonAlloc(player.transform.position, 3f, hitColliders, 1,
					QueryTriggerInteraction.UseGlobal);
				var diveHatch = hitColliders
					.Select(hitCollider => hitCollider.gameObject.GetComponentInParent<UseableDiveHatch>())
					.FirstOrDefault(hatch => hatch != null && hatch.isForWaterPark);

				if (diveHatch != null)
				{
					diveHatch.StartCinematicMode(diveHatch.enterCinematicController, player);
					if (diveHatch.enterCustomGoalText != "" && (!diveHatch.customGoalWithLootOnly ||
																Inventory.main.GetTotalItemCount() > 0))
					{
						Debug.Log("OnCustomGoalEvent(" + diveHatch.enterCustomText);
						GoalManager.main.OnCustomGoalEvent(diveHatch.enterCustomGoalText);
					}

					if (diveHatch.secureInventory)
					{
						Inventory.Get().SecureItems(true);
					}
				}
				ProcessMSG(msg3, false);
			}
	}

	private static void ClearMsgs()
	{
		ProcessMSG($"Attach as target override = {Main.Config.AttachToTarget}", false);
		ProcessMSG($"Full Override = {Main.Config.FullOverride}", false);
		Main.Config.AttachToTarget = false;
		Main.Config.FullOverride = false;
		ProcessMSG($"Attach as target override = {Main.Config.AttachToTarget}", false);
		ProcessMSG($"Full Override = {Main.Config.FullOverride}", false);
	}

	private static void ProcessMSG(string msg, bool active)
	{
		var message = ErrorMessage.main.GetExistingMessage(msg);
		if (active)
		{
			if (message != null)
			{
				message.messageText = msg;
				message.entry.text = msg;
				if (message.timeEnd <= Time.time + 1f)
					message.timeEnd += Time.deltaTime;
			}
			else
			{
				ErrorMessage.AddMessage(msg);
			}
		}
		else if (message != null && message.timeEnd > Time.time)
		{
			message.timeEnd = Time.time;
		}
	}
}
