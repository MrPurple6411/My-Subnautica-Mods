namespace BuildingTweaks.Patches;

using HarmonyLib;
using UnityEngine;
using Nautilus.Handlers;
using System.Linq;
using System;

[HarmonyPatch(typeof(Player), nameof(Player.Update))]
public static class Player_Update_Patch
{
	private static int _line = 0;

	[HarmonyPostfix]
	public static void Postfix(Player __instance)
	{
		try
		{
			_line = 19;
			PlayerTool heldTool = Inventory.main.GetHeldTool();
			_line = 21;
			Vehicle vehicle = __instance.GetVehicle();
			_line = 23;
			Pickupable module = vehicle?.GetSlotItem(vehicle.GetActiveSlotID())?.item;
			_line = 25;
			bool builderCheck = heldTool != null && heldTool.pickupable.GetTechType() == TechType.Builder;
			_line = 27;
			bool builderModuleCheck = EnumHandler.TryGetValue("BuilderModule", out TechType modTechType) && module != null && module.GetTechType() == modTechType;
			_line = 29;
			if (DevConsole.instance != null && !DevConsole.instance.state && (builderCheck || builderModuleCheck))
			{
				_line = 32;
				ProcessMSG($"Attach as target override = {Main.Config.AttachToTarget}", true);
				_line = 34;
				ProcessMSG($"Full Override = {Main.Config.FullOverride}", true);
				_line = 36;
				if (Input.GetKeyDown(Main.Config.AttachToTargetToggle))
				{
					_line = 39;
					ProcessMSG($"Attach as target override = {Main.Config.AttachToTarget}", false);
					_line = 41;
					Main.Config.AttachToTarget = !Main.Config.AttachToTarget;
					_line = 43;
					ProcessMSG($"Attach as target override = {Main.Config.AttachToTarget}", true);
				}
				_line = 46;
				if (Input.GetKeyDown(Main.Config.FullOverrideToggle))
				{
					_line = 49;
					ProcessMSG($"Full Override = {Main.Config.FullOverride}", false);
					_line = 51;
					Main.Config.FullOverride = !Main.Config.FullOverride;
					_line = 53;
					if (Builder.prefab != null && !Builder.canPlace)
					{
						_line = 56;
						var value = Main.Config.FullOverride ? Builder.placeColorAllow : Builder.placeColorDeny;
						_line = 58;
						var components = Builder.ghostModel.GetComponents<IBuilderGhostModel>();
						_line = 60;
						foreach (var builderGhostModel in components)
							builderGhostModel.UpdateGhostModelColor(true, ref value);
						_line = 63;
						Builder.ghostStructureMaterial.SetColor(ShaderPropertyID._Tint, value);
					}
					_line = 66;
					ProcessMSG($"Full Override = {Main.Config.FullOverride}", true);
				}
			}
			else
			{
				_line = 72;
				ClearMsgs();
			}

			_line = 76;
			var waterPark = __instance.currentWaterPark;
			_line = 78;
			if (waterPark != null && waterPark.GetComponentInParent<Creature>() != null)
			{
				_line = 81;
				var vector3 = __instance.currentWaterPark.transform.position;
				_line = 83;
				__instance.SetPosition(vector3);
				_line = 85;
				var msg3 = $"Press {GameInput.GetBinding(GameInput.GetPrimaryDevice(), GameInput.Button.Exit, GameInput.BindingSet.Primary)} to exit waterpark if you cant reach the exit.";
				_line = 87;
				if (!GameInput.GetButtonDown(GameInput.Button.Exit))
				{
					_line = 90;
					ProcessMSG(msg3, true);
					return;
				}
				_line = 94;
				Collider[] hitColliders = { };
				_line = 96;
				Physics.OverlapSphereNonAlloc(__instance.transform.position, 3f, hitColliders, 1,
					QueryTriggerInteraction.UseGlobal);
				_line = 99;
				var diveHatch = hitColliders
					.Select(hitCollider => hitCollider.gameObject.GetComponentInParent<UseableDiveHatch>())
					.FirstOrDefault(hatch => hatch != null && hatch.isForWaterPark);

				_line = 104;
				if (diveHatch != null)
				{
					_line = 107;
					diveHatch.StartCinematicMode(diveHatch.enterCinematicController, __instance);
					_line = 109;
					if (diveHatch.enterCustomGoalText != "" && (!diveHatch.customGoalWithLootOnly ||
																Inventory.main.GetTotalItemCount() > 0))
					{
						_line = 113;
						Debug.Log("OnCustomGoalEvent(" + diveHatch.enterCustomText);
						_line = 115;
						GoalManager.main.OnCustomGoalEvent(diveHatch.enterCustomGoalText);
					}

					_line = 119;
					if (diveHatch.secureInventory)
					{
						_line = 122;
						Inventory.Get().SecureItems(true);
					}
				}
				_line = 126;
				ProcessMSG(msg3, false);
			}

#if SUBNAUTICA
			_line = 131;
			var currentSubRoot = __instance.GetCurrentSub();
			_line = 133;
			if (currentSubRoot != null && currentSubRoot is BaseRoot && __instance.playerController.velocity.y < -20f)
			{
				_line = 136;
				var componentInChildren = currentSubRoot.gameObject.GetComponentInChildren<RespawnPoint>();
				_line = 138;
				if (componentInChildren)
				{
					_line = 141;
					__instance.SetPosition(componentInChildren.GetSpawnPosition());
					return;
				}
			}
			_line = 146;
			var escapePod = __instance.currentEscapePod;
			_line = 148;
			if (escapePod == null || !(__instance.playerController.velocity.y < -20f)) return;
			_line = 150;
			var transform = escapePod.playerSpawn.transform;
			_line = 152;
			__instance.SetPosition(transform.position, transform.rotation);
#elif BELOWZERO
        var interiorSpace = __instance.currentInterior;
        if (interiorSpace == null || !(__instance.playerController.velocity.y < -20f)) return;
        var respawnPoint = interiorSpace.GetRespawnPoint();
        if (!respawnPoint) return;
        __instance.SetPosition(respawnPoint.GetSpawnPosition());
#endif
		}
		catch (Exception ex)
		{
			Main.Logger.LogError($"Error in Player_Update_Patch at line {_line}: {ex.Message}");
			ProcessMSG($"Error in Player_Update_Patch at line {_line}: {ex.Message}", true);
			_line = 0;
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
