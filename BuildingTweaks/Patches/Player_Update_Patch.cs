using FMOD;
using HarmonyLib;
using SMLHelper.V2.Handlers;
using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BuildingTweaks.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    public static class Player_Update_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            PlayerTool heldTool = Inventory.main.GetHeldTool();
            Vehicle vehicle = __instance.GetVehicle();
            Pickupable module = vehicle?.GetSlotItem(vehicle.GetActiveSlotID())?.item;

            bool builderCheck = heldTool?.pickupable?.GetTechType() == TechType.Builder;
            bool builderModuleCheck = module != null && TechTypeHandler.TryGetModdedTechType("BuilderModule", out TechType modTechType) && module.GetTechType() == modTechType;

            string msg1 = $"Attach as target override = {Main.config.AttachToTarget}";
            ErrorMessage._Message emsg = ErrorMessage.main.GetExistingMessage(msg1);
            string msg2 = $"Full Override = {Main.config.FullOverride}";
            ErrorMessage._Message emsg2 = ErrorMessage.main.GetExistingMessage(msg2);

            if (DevConsole.instance != null && !DevConsole.instance.state && (builderCheck || builderModuleCheck))
            {
                if (Input.GetKeyDown(Main.config.AttachToTargetToggle))
                {
                    Main.config.AttachToTarget = !Main.config.AttachToTarget;
                    msg1 = $"Attach as target override = {Main.config.AttachToTarget}";
                }

                if (Input.GetKeyDown(Main.config.FullOverrideToggle))
                {
                    Main.config.FullOverride = !Main.config.FullOverride;
                    msg2 = $"Full Override = {Main.config.FullOverride}";

                }

                if (emsg != null)
                {
                    emsg.messageText = msg1;
                    emsg.entry.text = msg1;
                    if (emsg.timeEnd <= Time.time + 1f)
                        emsg.timeEnd += Time.deltaTime;
                }
                else
                    ErrorMessage.AddMessage(msg1);

                if (emsg2 != null)
                {
                    emsg2.messageText = msg2;
                    emsg2.entry.text = msg2;

                    if(emsg2.timeEnd <= Time.time + 1f)
                        emsg2.timeEnd += Time.deltaTime;
                }
                else
                    ErrorMessage.AddMessage(msg2);
            }
            else if( Main.config.AttachToTarget || Main.config.FullOverride)
            {
                Main.config.AttachToTarget = false;
                Main.config.FullOverride = false;

                if(emsg != null)
                    emsg.timeEnd = Time.time;

                if (emsg2 != null)
                    emsg2.timeEnd = Time.time;
            }

            WaterPark waterPark = __instance?.currentWaterPark;

            if (waterPark?.GetComponentInParent<Creature>() != null)
            {
                Vector3 vector3 = __instance.currentWaterPark.transform.position;
                __instance.SetPosition(vector3);

                string msg3 = $"Press {GameInput.GetBinding(GameInput.GetPrimaryDevice(), GameInput.Button.Exit, GameInput.BindingSet.Primary)} to exit waterpark if you cant reach the exit.";
                ErrorMessage._Message emsg3 = ErrorMessage.main.GetExistingMessage(msg3);
                if (emsg3 != null && emsg3.timeEnd <= Time.time + 1f)
                    emsg3.timeEnd += Time.deltaTime;
                else if(emsg3 is null)
                    ErrorMessage.AddMessage(msg3);

                if (GameInput.GetButtonDown(GameInput.Button.Exit))
                {
                    UseableDiveHatch diveHatch = null;

                    Collider[] hitColliders = Physics.OverlapSphere(__instance.transform.position, 3f, 1, QueryTriggerInteraction.UseGlobal);
                    foreach (Collider hitCollider in hitColliders)
                    {
                        UseableDiveHatch hatch = hitCollider.gameObject.GetComponentInParent<UseableDiveHatch>();
                        if (hatch != null && hatch.isForWaterPark)
                        {
                            diveHatch = hatch;
                            break;
                        }
                    }

                    if (diveHatch != null)
                    {
                        diveHatch.StartCinematicMode(diveHatch.enterCinematicController, __instance);
                        if (diveHatch.enterCustomGoalText != "" && (!diveHatch.customGoalWithLootOnly || Inventory.main.GetTotalItemCount() > 0))
                        {
                            Debug.Log("OnCustomGoalEvent(" + diveHatch.enterCustomText);
                            GoalManager.main.OnCustomGoalEvent(diveHatch.enterCustomGoalText);
                        }
                        if (diveHatch.secureInventory)
                        {
                            Inventory.Get().SecureItems(true);
                        }
                    }
                }
                return;
            }

#if SN1
            SubRoot currentSubRoot = __instance.GetCurrentSub();
            if (currentSubRoot != null && currentSubRoot is BaseRoot && __instance.playerController.velocity.y < -20f)
            {
                RespawnPoint componentInChildren = currentSubRoot.gameObject.GetComponentInChildren<RespawnPoint>();
                if (componentInChildren)
                {
                    __instance.SetPosition(componentInChildren.GetSpawnPosition());
                    return;
                }
            }

            EscapePod escapePod = __instance.currentEscapePod;
            if(escapePod != null && __instance.playerController.velocity.y < -20f)
            {
                __instance.SetPosition(escapePod.playerSpawn.transform.position, escapePod.playerSpawn.transform.rotation);
                return;
            }
#elif BZ
            IInteriorSpace interiorSpace = __instance.currentInterior;
            if (interiorSpace != null && __instance.playerController.velocity.y < -20f)
            {
                RespawnPoint respawnPoint = interiorSpace.GetRespawnPoint();
                if (respawnPoint)
                {
                    __instance.SetPosition(respawnPoint.GetSpawnPosition());
                    return;
                }
            }
#endif
        }
    }
}
