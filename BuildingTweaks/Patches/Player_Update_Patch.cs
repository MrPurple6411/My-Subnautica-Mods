namespace BuildingTweaks.Patches
{
    using HarmonyLib;
    using SMLHelper.V2.Handlers;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    public static class Player_Update_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if(Input.GetKeyDown(Main.Config.AttachToTargetToggle))
            {
                ProcessMSG($"Attach as target override = {Main.Config.AttachToTarget}", false);
                Main.Config.AttachToTarget = !Main.Config.AttachToTarget;
                ProcessMSG($"Attach as target override = {Main.Config.AttachToTarget}", true);
            }

            if(Input.GetKeyDown(Main.Config.FullOverrideToggle))
            {
                ProcessMSG($"Full Override = {Main.Config.FullOverride}", false);
                Main.Config.FullOverride = !Main.Config.FullOverride;
                if(Main.Config.FullOverride && Builder.prefab != null && !Builder.canPlace)
                {
                    Builder.canPlace = true;
                    Color value = Builder.placeColorAllow;
                    IBuilderGhostModel[] components = Builder.ghostModel.GetComponents<IBuilderGhostModel>();
                    for(int i = 0; i < components.Length; i++)
                    {
                        components[i].UpdateGhostModelColor(true, ref value);
                    }
                    Builder.ghostStructureMaterial.SetColor(ShaderPropertyID._Tint, value);
                }
                ProcessMSG($"Full Override = {Main.Config.FullOverride}", true);
            }

            PlayerTool heldTool = Inventory.main.GetHeldTool();

            bool builderCheck = heldTool?.pickupable != null && heldTool.pickupable.GetTechType() == TechType.Builder;
            bool builderModuleCheck = false;

            if(!builderCheck && Builder.prefab != null)
            {
                if(__instance.GetVehicle()!= null)
                    builderModuleCheck = true;
#if BZ
                else if(Player.main.IsPilotingSeatruck())
                    builderModuleCheck = true;
                else if(Player.main.inHovercraft)
                    builderModuleCheck = true;
#endif
            }

            if(builderCheck || builderModuleCheck)
            {
                if(Input.GetMouseButtonDown(2))
                    Builder_Update_Patches.Freeze = !Builder_Update_Patches.Freeze;

                if(Builder_Update_Patches.Freeze && Input.GetKeyDown(KeyCode.H))
                    Builder.UpdateAllowed();
            }
            else
            {
                ClearMsgs();
            }

            WaterPark waterPark = __instance?.currentWaterPark;

            if(waterPark?.GetComponentInParent<Creature>() != null)
            {
                Vector3 vector3 = __instance.currentWaterPark.transform.position;
                __instance.SetPosition(vector3);

                string msg3 = $"Press {GameInput.GetBinding(GameInput.GetPrimaryDevice(), GameInput.Button.Exit, GameInput.BindingSet.Primary)} to exit waterpark if you cant reach the exit.";

                if(GameInput.GetButtonDown(GameInput.Button.Exit))
                {
                    UseableDiveHatch diveHatch = null;

                    Collider[] hitColliders = Physics.OverlapSphere(__instance.transform.position, 3f, 1, QueryTriggerInteraction.UseGlobal);
                    foreach(Collider hitCollider in hitColliders)
                    {
                        UseableDiveHatch hatch = hitCollider.gameObject.GetComponentInParent<UseableDiveHatch>();
                        if(hatch != null && hatch.isForWaterPark)
                        {
                            diveHatch = hatch;
                            break;
                        }
                    }

                    if(diveHatch != null)
                    {
                        diveHatch.StartCinematicMode(diveHatch.enterCinematicController, __instance);
                        if(diveHatch.enterCustomGoalText != "" && (!diveHatch.customGoalWithLootOnly || Inventory.main.GetTotalItemCount() > 0))
                        {
                            Debug.Log("OnCustomGoalEvent(" + diveHatch.enterCustomText);
                            GoalManager.main.OnCustomGoalEvent(diveHatch.enterCustomGoalText);
                        }
                        if(diveHatch.secureInventory)
                        {
                            Inventory.Get().SecureItems(true);
                        }
                    }
                    ProcessMSG(msg3, false);
                }
                else
                {
                    ProcessMSG(msg3, true);
                }
                return;
            }

#if SN1
            SubRoot currentSubRoot = __instance.GetCurrentSub();
            if(currentSubRoot != null && currentSubRoot is BaseRoot && __instance.playerController.velocity.y < -20f)
            {
                RespawnPoint componentInChildren = currentSubRoot.gameObject.GetComponentInChildren<RespawnPoint>();
                if(componentInChildren)
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

        private static void ClearMsgs()
        {
            ProcessMSG($"Attach as target override = {Main.Config.AttachToTarget}", false);
            ProcessMSG($"Full Override = {Main.Config.FullOverride}", false);
            Main.Config.AttachToTarget = false;
            Main.Config.FullOverride = false;
            ProcessMSG($"Attach as target override = {Main.Config.AttachToTarget}", false);
            ProcessMSG($"Full Override = {Main.Config.FullOverride}", false);
        }

        internal static void ProcessMSG(string msg, bool active)
        {
            ErrorMessage._Message emsg = ErrorMessage.main.GetExistingMessage(msg);
            if(active)
            {
                if(emsg != null)
                {
                    emsg.messageText = msg;
                    emsg.entry.text = msg;
                    if(emsg.timeEnd <= Time.time + 1f)
                        emsg.timeEnd += Time.deltaTime;
                }
                else
                {
                    ErrorMessage.AddMessage(msg);
                }
            }
            else if(emsg != null && emsg.timeEnd > Time.time)
            {
                emsg.timeEnd = Time.time;
            }
        }
    }
}
